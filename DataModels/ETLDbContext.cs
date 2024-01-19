using Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domains;

public class ETLDbContext : DbContext
{
	public DbSet<Categories> Categories { get; set; }
	public DbSet<Orders> Orders { get; set; }
	public DbSet<Products> Products { get; set; }
	public ETLDbContext(DbContextOptions<ETLDbContext> options) : base(options) { }

	public void EnableCDC()
	{
		Database.ExecuteSqlRaw("EXEC sys.sp_cdc_enable_db");
		Database.ExecuteSqlRaw(@$"EXEC sys.sp_cdc_enable_table
								  @source_schema = 'dbo',
								  @source_name = '{nameof(Categories)}',
								  @role_name = NULL,
								  @supports_net_changes = 1");
		Database.ExecuteSqlRaw(@$"EXEC sys.sp_cdc_enable_table
								  @source_schema = 'dbo',
								  @source_name = '{nameof(Orders)}',
								  @role_name = NULL,
								  @supports_net_changes = 1");
		Database.ExecuteSqlRaw(@$"EXEC sys.sp_cdc_enable_table
								  @source_schema = 'dbo',
								  @source_name = '{nameof(Products)}',
								  @role_name = NULL,
								  @supports_net_changes = 1");
		Database.ExecuteSqlRaw(@$"EXEC sys.sp_cdc_enable_table
								  @source_schema = 'dbo',
								  @source_name = '{nameof(Orders)}{nameof(Products)}',
								  @role_name = NULL,
								  @supports_net_changes = 1");
	}
	public void EnableChangeTracking()
	{

		Database.ExecuteSqlRaw(@$"ALTER DATABASE destETLDB SET CHANGE_TRACKING = ON  
								  (CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON)");
		Database.ExecuteSqlRaw(@$"ALTER TABLE {nameof(Products)}
								  ENABLE CHANGE_TRACKING  
								  WITH (TRACK_COLUMNS_UPDATED = ON)");
	}
}
