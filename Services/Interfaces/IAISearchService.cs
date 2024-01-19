namespace Services.Interfaces;

public interface IAISearchService
{
	Task<string> GetBeautifiedSearchResultsAsync(string indexName, string query);
}
