using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace Turrab.ETLTask.Core.Services.UIServices;

public class MainMenuService
{
    private readonly IServiceProvider _serviceProvider;
    public MainMenuService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunAsync()
        => await CommonsService.ProcessMenuAsync(
            displayMenuDelegate: DisplayMenu,
            processMenuSelectionAsyncDelegate: InvokeSelectedServiceAsync,
            minSelectionLimit: 1,
            exitSelection: 4,
            logger: LogManager.GetCurrentClassLogger());
    private void DisplayMenu()
        => CommonsService.DisplayMenuTable(
            new List<string> {
                "Datafactory options",
                "Search Products",
                "Apply Migrations",
                "Exit" });

    private async Task InvokeSelectedServiceAsync(int selection)
    {
        switch (selection)
        {
            case 1:
                _serviceProvider.GetRequiredService<DataFactoryOptionsService>().Run();
                break;
            case 2:
                await _serviceProvider.GetRequiredService<SearchProductsService>().RunAsync();
                break;
            case 3:
                await _serviceProvider.GetRequiredService<ApplyMigrationsService>().RunAsync();
                break;
        }
    }
}