using Core.Services;
using NLog;
using Services.Interfaces;

namespace Turrab.ETLTask.Core.Services.UIServices;

public class ApplyMigrationsService
{
	private readonly IMigrationService _migrationService;
	private readonly Logger _logger;
	public ApplyMigrationsService(IMigrationService migrationService)
	{
		_migrationService = migrationService ?? throw new ArgumentNullException(nameof(migrationService));
		_logger = LogManager.GetCurrentClassLogger();
	}
	public async Task RunAsync()
	{
		try
		{
			Console.WriteLine("Applying Migrations. Please wait...");
			_logger.Info("Applying Migrations. Please wait...");
			var messages = await _migrationService.ApplyMigration();
			Console.Clear();
			CommonsService.DisplayMessage("Migrations completed!", messages, new List<string> { "Message", "Status" });
			_logger.Info($"Migrations completed!\n {string.Join(" ", messages)}");
			Console.WriteLine();
			try
			{
				Console.WriteLine("Applying CDC to tables. Please wait...");
				_logger.Info("Applying CDC to tables. Please wait...");
				_migrationService.EnableCDC();
				CommonsService.DisplayMessage("CDC Configured Successfully!", new List<string>(), new List<string>());
				_logger.Info("CDC Configured Successfully!");
			}
			catch (Exception ex)
			{
				CommonsService.DisplayErrorMessage(ex: ex, title: "CDC Error", logger: _logger);
			}
			try
			{
				Console.WriteLine("Enabling Change Tracking to Products. Please wait...");
				_logger.Info("Enabling Change Tracking to Products. Please wait...");
				_migrationService.EnableChangeTracking();
				CommonsService.DisplayMessage("Change Tracking To Products Configured Successfully!", new List<string>(), new List<string>());
				_logger.Info("CDC Configured Successfully!");
			}
			catch (Exception ex)
			{
				CommonsService.DisplayErrorMessage(ex: ex, title: "Change Tracking Error", logger: _logger);
			}
		}
		catch (Exception ex)
		{
			CommonsService.DisplayErrorMessage(ex: ex, title: "Migrations Error", logger: _logger);
		}
		CommonsService.WaitForKeyPress();
	}
}
