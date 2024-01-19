using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Domains.ConfigModels;
using Services.Interfaces;

namespace Services.Classes;

public class AzureKeyVaultService : IAzureKeyVaultService
{
	private readonly SecretClient _secretClient;
	public AzureKeyVaultService(AzureClient azureClient)
	{
		var uri = new Uri($"https://{azureClient.KeyVaultName}.vault.azure.net");
		_secretClient = new SecretClient(uri, new DefaultAzureCredential());
	}

	public async Task<string> GetSecretAsync(string secretName)
	{
		var secret = await _secretClient.GetSecretAsync(secretName);
		if (secret == null) throw new InvalidOperationException("Secret not found!");
		return secret.Value.Value;
	}
}
