using Domains.ConfigModels;
using Newtonsoft.Json;
using Services.Interfaces;

namespace Services.Classes;

public class AISearchService : IAISearchService
{
	private readonly AISearchClient _aiSearchClient;
	private readonly HttpClient _httpClient;
	public AISearchService(AzureClient azureClient)
	{
		_aiSearchClient = azureClient.AISearchClient;
		_httpClient = new HttpClient();
	}

	public async Task<string> GetBeautifiedSearchResultsAsync(string indexName, string query)
	{
		var url = ConstructAISearchUrl(indexName, query);
		var response = await GetSearchResponseAsync(url);
		var body = await response.Content.ReadAsStringAsync();
		if (!response.IsSuccessStatusCode)
		{
			Console.WriteLine("There is some issues with the response : ");
			Console.WriteLine($"Status Code : {response.StatusCode}");
			Console.WriteLine($"Message : {body}");
			return "";
		}
		var searchResult = JsonConvert.DeserializeObject<SearchResult>(body);
		string beautifiedSearch = JsonConvert.SerializeObject(searchResult.Value, Formatting.Indented);
		return beautifiedSearch;
	}
	private async Task<HttpResponseMessage> GetSearchResponseAsync(string url)
	{
		var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
		requestMessage.Headers.Add("Accept", "application/json");
		requestMessage.Headers.Add("api-key", _aiSearchClient.ApiKey);
		return await _httpClient.SendAsync(requestMessage);
	}
	private string ConstructAISearchUrl(string indexName, string searchQuery)
		=> $"https://{_aiSearchClient.SearchService}.search.windows.net/indexes/{indexName}/docs?search={searchQuery}&$count=true&api-version={_aiSearchClient.ApiVersion}";

	private class SearchResult
	{
		public string ODataContext { get; set; }
		public int ODataCount { get; set; }
		public List<Category> Value { get; set; }
		public string ODataNextLink { get; set; }
	}

	private class Category
	{
		public double SearchScore { get; set; }
		public string CategoryId { get; set; }
		public string CategoryName { get; set; }
	}
}
