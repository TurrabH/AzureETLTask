using Domains;
using Domains.ConfigModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Classes;
using Services.Interfaces;
using Turrab.ETLTask.Core.Services.UIServices;

namespace Core.Extensions;

public static class ServiceExtensions
{
	public static ServiceCollection RegisterConfigurationServices(this ServiceCollection services)
	{
		var appSettingsStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Turrab.ETLTask.Core.appsettings.json");
		var configuration = new ConfigurationBuilder().AddJsonStream(appSettingsStream ?? throw new InvalidOperationException(message: "appSettings.json not found.")).Build();
		services.AddSingleton<IConfiguration>(configuration);
		var azureClient = new AzureClient();
		configuration.GetSection("AzureClient").Bind(azureClient);
		services.AddSingleton<AzureClient>(implementationFactory: provider => azureClient);
		services.AddSingleton(implementationFactory: provider => new ETLDbContext(new DbContextOptionsBuilder<ETLDbContext>().UseSqlServer(connectionString: configuration["Azure:SQLSrc"]).Options));
		return services;
	}
	public static ServiceCollection RegisterBaseServices(this ServiceCollection services)
	{
		services.AddSingleton<IMigrationService, MigrationService>();
		services.AddSingleton<IDataFactoryService, DataFactoryService>();
		services.AddSingleton<IAISearchService, AISearchService>();
		services.AddSingleton<IAzureKeyVaultService, AzureKeyVaultService>();
		services.AddSingleton<ApplyMigrationsService>();
		services.AddSingleton<DataFactoryOptionsService>();
		services.AddSingleton<SearchProductsService>();
		return services;
	}
}
