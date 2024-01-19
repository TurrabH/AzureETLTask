using Domains;
using Domains.ConfigModels;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;

namespace Services.Classes;

public class MigrationService : IMigrationService
{
	private readonly ETLDbContext _srcDbContext;
	private readonly ETLDbContext _destDbContext;

	public MigrationService(AzureClient azureClient)
		=> (_srcDbContext, _destDbContext) = GetDBContextConfig(azureClient ?? throw new ArgumentNullException(nameof(azureClient)));

	public async Task<List<string>> ApplyMigration()
	{
		var srcCreated = await _srcDbContext.Database.EnsureCreatedAsync();
		var destCreated = await _destDbContext.Database.EnsureCreatedAsync();
		return new List<string>
		{
			$"Source Created, {srcCreated}",
			$"Destination Created, {destCreated}",
		};
	}

	public void EnableCDC()
	{
		_srcDbContext.EnableCDC();
		_destDbContext.EnableCDC();
	}

	public void EnableChangeTracking()
	{
		_destDbContext.EnableChangeTracking();
	}

	private (ETLDbContext, ETLDbContext) GetDBContextConfig(AzureClient azureClient)
	{
		var srcDBContextOptionsBuilder = new DbContextOptionsBuilder<ETLDbContext>().UseSqlServer(connectionString: azureClient.SrcDb, builder => builder.EnableRetryOnFailure());
		var destDBContextOptionsBuilder = new DbContextOptionsBuilder<ETLDbContext>().UseSqlServer(connectionString: azureClient.DestDb, builder => builder.EnableRetryOnFailure());
		var srcDbContext = new ETLDbContext(srcDBContextOptionsBuilder.Options);
		var destDbContext = new ETLDbContext(destDBContextOptionsBuilder.Options);
		return (srcDbContext, destDbContext);
	}
}
