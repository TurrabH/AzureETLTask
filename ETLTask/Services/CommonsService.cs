using ConsoleTables;
using NLog;

namespace Core.Services;

public static class CommonsService
{
	public delegate void RunFunc();
	public delegate void DisplayMenu();
	public delegate Task ProcessMenuAsyncSelection(int selection);
	public delegate void ProcessMenuSelection(int selection);
	public static void WaitForKeyPress()
	{
		Console.WriteLine("\n\nPress any key to continue!");
		Console.ReadKey();
		Console.Clear();
	}

	public static async Task ProcessMenuAsync(
		DisplayMenu displayMenuDelegate,
		ProcessMenuAsyncSelection processMenuSelectionAsyncDelegate,
		int minSelectionLimit,
		int exitSelection,
		Logger logger)
	{
		do
		{
			displayMenuDelegate();
			var selection = GetMenuInput(min: minSelectionLimit, max: exitSelection, error: out var error, logger: logger);
			if (error) continue;
			if (selection == exitSelection) break;
			await processMenuSelectionAsyncDelegate(selection);
		} while (true);
	}

	public static void ProcessMenu(
		DisplayMenu displayMenuDelegate,
		ProcessMenuSelection processMenuSelectionDelegate,
		int minSelectionLimit,
		int exitSelection,
		Logger logger)
	{
		do
		{
			displayMenuDelegate();
			var selection = GetMenuInput(min: minSelectionLimit, max: exitSelection, error: out var error, logger: logger);
			if (error) continue;
			if (selection == exitSelection) break;
			processMenuSelectionDelegate(selection);
		} while (true);
	}
	public static int GetMenuInput(int min, int max, out bool error, Logger logger)
	{
		error = false;
		Console.Write($"Please select an option between {min}-{max}: ");
		var parsed = int.TryParse(Console.ReadLine(), out var selection);
		Console.Clear();
		if (!parsed)
		{
			DisplayErrorMessage(ex: new IOException(message: "Try an 'int' value."), title: "Input Error", logger: logger);
			error = true;
		}
		else if (selection < min || selection > max)
		{
			DisplayErrorMessage(ex: new IOException(message: $"Please select a value in range ({min}-{max})"), title: "Input Error", logger: logger);
			error = true;
		}
		return selection;
	}

	public static void DisplayErrorMessage(string title, Exception ex, Logger logger)
	{
		logger.Error(title, ex);
		int titleLength = title.Length + 4;
		string dashes = new('-', titleLength);

		Console.WriteLine($" {dashes}");
		Console.WriteLine($"   {title.ToUpper()}  ");
		Console.WriteLine($" {dashes}");

		var table = new ConsoleTable("Error Type", "Message");
		table.AddRow(ex.GetType().Name, WrapText(ex.Message, 50));
		table.Write();

		Console.WriteLine($" {dashes}");
	}

	public static void DisplayMessage(string title, List<string> rows, List<string> columns)
	{
		int titleLength = title.Length + 4;
		string dashes = new('-', titleLength);

		Console.WriteLine($" {dashes}");
		Console.WriteLine($"   {title.ToUpper()}  ");
		Console.WriteLine($" {dashes}");

		var table = new ConsoleTable(columns.ToArray());

		foreach (var row in rows)
		{
			var wrappedRows = row.Split(',').Select(x => WrapText(x, 50));
			table.AddRow(wrappedRows.ToArray());
		}
		table.Write();

		Console.WriteLine($" {dashes}");
	}

	public static void DisplayMenuTable(List<string> options)
	{
		var table = new ConsoleTable("#", "Option");
		int index = 1;
		foreach (var row in options) table.AddRow(index++, row);
		table.Write(Format.Minimal);
	}

	private static string WrapText(string text, int maxLength)
	{
		if (text.Length <= maxLength)
			return text;
		else
			return text.Substring(0, maxLength - 3) + "...";
	}
}
