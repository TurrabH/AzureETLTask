namespace Services.Interfaces;

public interface IAzureKeyVaultService
{
	Task<string> GetSecretAsync(string secretName);
}
