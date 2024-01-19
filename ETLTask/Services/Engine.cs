using Domains.ConfigModels;
using Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces;
using Turrab.ETLTask.Core.Services.UIServices;
using Bogus;
using Domains.Entities;
using Domains;
using NLog;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class Engine
{

	private readonly MainMenuService _mainMenuService;
	private readonly IServiceProvider _serviceProvider;
	private readonly Logger _logger;
	public Engine()
	{
		_serviceProvider = new ServiceCollection()
			.RegisterConfigurationServices()
			.RegisterBaseServices()
			.BuildServiceProvider();
		_mainMenuService = new MainMenuService(_serviceProvider);
		_logger = LogManager.GetCurrentClassLogger();
		Console.Clear();
	}

	public async Task RunAsync()
	{
		try
		{
			await ConfigureKeyVaultValuesAsync();
			await _mainMenuService.RunAsync();
		}
		catch (Exception ex)
		{
			CommonsService.DisplayErrorMessage("App Error", ex, _logger);
		}
	}

	public async Task PopulateData()
	{
		try
		{
			await ConfigureKeyVaultValuesAsync();
			var azureClient = _serviceProvider.GetRequiredService<AzureClient>();
			var srcDBContextOptionsBuilder = new DbContextOptionsBuilder<ETLDbContext>().UseSqlServer(connectionString: azureClient.SrcDb, builder => builder.EnableRetryOnFailure());
			var dbContext = new ETLDbContext(srcDBContextOptionsBuilder.Options);
			var random = new Random();
			var recordsCount = 100;
			var relationCount = recordsCount - (recordsCount * 10 / 100);
			var skipAndTake = relationCount - (relationCount * 10 / 100);
			dbContext.Orders.Include(o => o.Products);
			dbContext.Products.Include(p => p.Orders);
			Console.WriteLine("Loading data for orders");
			var orders = new Faker<Orders>()
				.RuleFor(o => o.OrderId, f => f.IndexFaker + 1)
				.RuleFor(o => o.OrderDate, f => f.Date.Past())
				.RuleFor(o => o.CustomerName, f => f.Name.FullName())
				.Generate(recordsCount);

			Console.WriteLine("Loading data for categories");
			var categories = new Faker<Categories>()
				.RuleFor(c => c.CategoryId, f => f.IndexFaker + 1)
				.RuleFor(c => c.CategoryName, f => f.Commerce.Department())
				.Generate(recordsCount);

			Console.WriteLine("Loading data for products");
			var products = new Faker<Products>()
				.RuleFor(p => p.ProductId, f => f.IndexFaker + 1)
				.RuleFor(p => p.ProductName, f => f.Commerce.ProductName())
				.RuleFor(p => p.CategoryId, f => f.PickRandom(categories).CategoryId)
				.RuleFor(p => p.Price, f => f.Finance.Amount(10, 1000))
				.RuleFor(p => p.ProductDescription, f => f.Lorem.Sentence())
				.RuleFor(p => p.ImageUrl, f => f.Image.PicsumUrl())
				.RuleFor(p => p.DateAdded, f => f.Date.Past())
				.Generate(recordsCount);

			Console.WriteLine("Publishing data for orders. Please wait...");
			await dbContext.Orders.AddRangeAsync(orders);

			Console.WriteLine("Publishing data for products. Please wait...");
			await dbContext.Products.AddRangeAsync(products);

			Console.WriteLine("Publishing data for categories. Please wait...");
			await dbContext.Categories.AddRangeAsync(categories);

			Console.WriteLine("Saving changes. Please wait...");
			await dbContext.SaveChangesAsync();

			Console.WriteLine("Configuring relations for products orders");
			dbContext.Products.Skip(random.Next(1, orders.Count)).Take(random.Next(1, relationCount)).ToList().ForEach(p =>
			{
				p.Orders ??= new List<Orders>();
				var randomOrders = dbContext.Orders.Skip(random.Next(1, orders.Count)).Take(random.Next(1, skipAndTake));
				foreach (var order in randomOrders) p.Orders.Add(order);
				dbContext.Entry(p).State = EntityState.Modified;
			});

			Console.WriteLine("Configuring relations for orders products");
			orders.Skip(random.Next(1, orders.Count)).Take(random.Next(1, relationCount)).ToList().ForEach(o =>
			{
				o.Products ??= new List<Products>();
				var randomProducts = products.Skip(random.Next(1, products.Count)).Take(random.Next(1, skipAndTake));
				foreach (var product in randomProducts) o.Products.Add(product);
				dbContext.Entry(o).State = EntityState.Modified;
			});
			Console.WriteLine("Saving changes. Please wait...");
			await dbContext.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			CommonsService.DisplayErrorMessage("Data Population Error", ex, _logger);
		}
	}

	public async Task ConfigureKeyVaultValuesAsync()
	{
		Console.WriteLine("Configuring KeyVault. Please wait...");
		var keyVaultService = _serviceProvider.GetRequiredService<IAzureKeyVaultService>();
		var azureClient = _serviceProvider.GetRequiredService<AzureClient>();
		var sqlUID = await keyVaultService.GetSecretAsync("SqlUID");
		var sqlPWD = await keyVaultService.GetSecretAsync("SQLPwd");
		azureClient.SrcDb = string.Format(azureClient.SrcDb, sqlUID, sqlPWD);
		azureClient.DestDb = string.Format(azureClient.DestDb, sqlUID, sqlPWD);
		azureClient.TenantId = await keyVaultService.GetSecretAsync("Tenant");
		azureClient.SubscriptionId = await keyVaultService.GetSecretAsync("Subscription");
		azureClient.AISearchClient.ApiKey = await keyVaultService.GetSecretAsync("APIKey");
		Console.Clear();
	}
}
