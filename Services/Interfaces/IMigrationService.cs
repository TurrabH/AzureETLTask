namespace Services.Interfaces;

public interface IMigrationService
{
	Task<List<string>> ApplyMigration();
	public void EnableCDC();
	void EnableChangeTracking();
}
