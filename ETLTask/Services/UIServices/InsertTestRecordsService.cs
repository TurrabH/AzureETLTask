using Bogus;
using Core.Services;
using Domains.ConfigModels;
using Domains.Entities;
using Domains;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Turrab.ETLTask.Core.Services.UIServices;

public class InsertTestRecordsService
{
	private readonly ETLDbContext _dbContext;
	private readonly Random _random;
	private readonly Logger _logger;
	private readonly int _recordsCount;
	private readonly int _relationCount;
	private readonly int _skipAndTake;
	public InsertTestRecordsService(AzureClient azureClient)
	{
		var srcDBContextOptionsBuilder = new DbContextOptionsBuilder<ETLDbContext>()
			.UseSqlServer(connectionString: azureClient?.SrcDb ?? throw new ArgumentNullException(nameof(azureClient)), builder => builder.EnableRetryOnFailure());
		_dbContext = new ETLDbContext(srcDBContextOptionsBuilder.Options);
		_random = new Random();
		_recordsCount = 100;
		_relationCount = _recordsCount - (_recordsCount * 10 / 100);
		_skipAndTake = _relationCount - (_relationCount * 10 / 100);
		_logger = LogManager.GetCurrentClassLogger();
		_dbContext.Orders.Include(o => o.Products);
		_dbContext.Products.Include(p => p.Orders);
	}

	public async Task RunAsync()
		=> await PopulateData();

	private async Task PopulateData()
	{
		try
		{

			Console.WriteLine("Loading data for orders. Please wait...");
			var orderLastIndex = _dbContext.Orders.OrderBy(x => x.OrderId).LastOrDefault()?.OrderId + 1 ?? 1;
			var orders = new Faker<Orders>()
				.RuleFor(o => o.OrderId, f => f.IndexFaker + orderLastIndex)
				.RuleFor(o => o.OrderDate, f => f.Date.Past())
				.RuleFor(o => o.CustomerName, f => f.Name.FullName())
				.Generate(_recordsCount);

			Console.WriteLine("Loading data for categories. Please wait...");
			var categoryLastIndex = _dbContext.Categories.OrderBy(x => x.CategoryId).LastOrDefault()?.CategoryId + 1 ?? 1;
			var categories = new Faker<Categories>()
				.RuleFor(c => c.CategoryId, f => f.IndexFaker + categoryLastIndex)
				.RuleFor(c => c.CategoryName, f => f.Commerce.Department())
				.Generate(_recordsCount);

			Console.WriteLine("Loading data for products. Please wait...");
			var productLastIndex = _dbContext.Products.OrderBy(x => x.ProductId).LastOrDefault()?.ProductId + 1 ?? 1;
			var products = new Faker<Products>()
				.RuleFor(p => p.ProductId, f => f.IndexFaker + productLastIndex)
				.RuleFor(p => p.ProductName, f => f.Commerce.ProductName())
				.RuleFor(p => p.CategoryId, f => f.PickRandom(categories).CategoryId)
				.RuleFor(p => p.Price, f => f.Finance.Amount(10, 1000))
				.RuleFor(p => p.ProductDescription, f => f.Lorem.Sentence())
				.RuleFor(p => p.ImageUrl, f => f.Image.PicsumUrl())
				.RuleFor(p => p.DateAdded, f => f.Date.Past())
				.Generate(_recordsCount);

			Console.WriteLine("Publishing data for orders. Please wait...");
			await _dbContext.Orders.AddRangeAsync(orders);

			Console.WriteLine("Publishing data for products. Please wait...");
			await _dbContext.Products.AddRangeAsync(products);

			Console.WriteLine("Publishing data for categories. Please wait...");
			await _dbContext.Categories.AddRangeAsync(categories);

			Console.WriteLine("Saving changes. Please wait...");
			await _dbContext.SaveChangesAsync();

			Console.WriteLine("Configuring relations for products orders. Please wait...");
			products.Skip(_random.Next(1, orders.Count)).Take(_random.Next(1, _relationCount)).ToList().ForEach(p =>
			{
				p.Orders ??= new List<Orders>();
				var randomOrders = orders.Skip(_random.Next(1, orders.Count)).Take(_random.Next(1, _skipAndTake));
				foreach (var order in randomOrders) p.Orders.Add(order);
				_dbContext.Entry(p).State = EntityState.Modified;
			});

			Console.WriteLine("Configuring relations for orders products. Please wait...");
			orders.Skip(_random.Next(1, orders.Count)).Take(_random.Next(1, _relationCount)).ToList().ForEach(o =>
			{
				o.Products ??= new List<Products>();
				var randomProducts = products.Skip(_random.Next(1, products.Count)).Take(_random.Next(1, _skipAndTake));
				foreach (var product in randomProducts) o.Products.Add(product);
				_dbContext.Entry(o).State = EntityState.Modified;
			});
			Console.WriteLine("Saving changes. Please wait...");
			await _dbContext.SaveChangesAsync();
			Console.WriteLine($"{_recordsCount} records inserted successfully!");
		}
		catch (Exception ex)
		{
			CommonsService.DisplayErrorMessage("Data Population Error", ex, _logger);
		}
		CommonsService.WaitForKeyPress();
	}
}
