using Core.Services;
using Domains.ConfigModels;
using NLog;
using Services.Interfaces;

namespace Turrab.ETLTask.Core.Services.UIServices;

public class SearchProductsService
{
    private readonly IAISearchService _aiSearchService;
    private readonly AISearchClient _aiSearchClient;
    private readonly Logger _logger;
    public SearchProductsService(IAISearchService aiSearchService, AzureClient azureClient)
    {
        _aiSearchService = aiSearchService ?? throw new ArgumentNullException(nameof(aiSearchService));
        _aiSearchClient = azureClient.AISearchClient ?? throw new ArgumentNullException(nameof(azureClient));
        _logger = LogManager.GetCurrentClassLogger();
    }

    public async Task RunAsync()
    {
        do
        {
            try
            {
                Console.WriteLine("==== Type q or quit to exit search ====");
                Console.Write("Write search query for the Products: ");
                var query = Console.ReadLine();
                if (string.IsNullOrEmpty(query)) query = "*";
                else if (query.Equals("q", StringComparison.InvariantCultureIgnoreCase) || query.Equals("quit", StringComparison.InvariantCultureIgnoreCase)) break;
                Console.Clear();
                Console.WriteLine("Fetching search results. Please wait...");
                _logger.Info("Fetching search results. Please wait...");
                var searchResults = await _aiSearchService.GetBeautifiedSearchResultsAsync(indexName: _aiSearchClient.ProductsIndex, query: query);
                Console.Clear();
                Console.WriteLine($"Search Query : {query}");
                _logger.Info($"Search Query : {query}");
                Console.WriteLine("Search results :");
                Console.WriteLine(searchResults);
                CommonsService.WaitForKeyPress();
            }
            catch (Exception ex)
            {
                CommonsService.DisplayErrorMessage("Search Exception", ex, logger: _logger);
            }
        } while (true);
        Console.Clear();
    }
}
