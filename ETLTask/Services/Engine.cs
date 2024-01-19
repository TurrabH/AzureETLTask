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
