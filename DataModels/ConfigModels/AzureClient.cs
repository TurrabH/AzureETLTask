namespace Domains.ConfigModels;

public class AzureClient
{
	public string SrcDb { get; set; }
	public string DestDb { get; set; }
	public string TenantId { get; set; }
	public string SubscriptionId { get; set; }
	public string[] Scopes { get; set; }
	public string KeyVaultName { get; set; }
	public DataFactoryClient DataFactoryClient { get; set; }
	public AISearchClient AISearchClient { get; set; }
}
