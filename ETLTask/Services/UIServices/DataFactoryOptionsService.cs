using Core.Services;
using NLog;
using Services.Interfaces;

namespace Turrab.ETLTask.Core.Services.UIServices;

public class DataFactoryOptionsService
{

    private readonly IDataFactoryService _azureService;
    private readonly Logger _logger;
    public DataFactoryOptionsService(IDataFactoryService azureService)
    {
        _azureService = azureService;
        _logger = LogManager.GetCurrentClassLogger();
    }
    public void Run()
        => CommonsService.ProcessMenu(
            displayMenuDelegate: DisplayMenu,
            processMenuSelectionDelegate: ProcessSelection,
            minSelectionLimit: 1,
            exitSelection: 4,
            logger: _logger);

    private void DisplayMenu()
        => CommonsService.DisplayMenuTable(
            new List<string> {
                "Trigger Pipeline",
                "Monitor Pipeline",
                "Dispose Pipeline",
                "Back" });

    private void ProcessSelection(int selection)
    {
        switch (selection)
        {
            case 1:
                TriggerPipeline();
                break;
            case 2:
                MonitorPipeline();
                break;
            case 3:
                DisposePipeline();
                break;
        }
        CommonsService.WaitForKeyPress();
    }

    private void DrawPipelineState(TimeSpan elapsedTime, string status)
    {
        Console.WriteLine("┌──────────┐");
        Console.WriteLine($"│ {elapsedTime:hh\\:mm\\:ss} │");
        Console.WriteLine("└──────────┘");
        Console.WriteLine($"Status: {status}");
    }
    private void TriggerPipeline()
    {
        try
        {
            Console.WriteLine("Triggering pipeline....");
            _logger.Info("Triggering pipeline....");
            _azureService.TriggerPipeline();
            Console.WriteLine("Pipeline Triggered Successfully!");
            _logger.Info("Pipeline Triggered Successfully!");
        }
        catch (Exception ex)
        {
            CommonsService.DisplayErrorMessage("Pipeline Failed", ex, logger: _logger);
        }
    }
    private void MonitorPipeline()
    {
        if (!_azureService.IsPipelineConfigured())
        {
            CommonsService.DisplayErrorMessage("Pipeline Error", new ArgumentException("Please start a pipeline to monitor!"), logger: _logger);
            return;
        }
        try
        {
            var currStatus = "...";
            var pipelineCompleted = false;
            var startTime = DateTime.Now;
            _ = Task.Run(async () =>
            {
                await foreach (var status in _azureService.MonitorPipeline()) currStatus = status;
                pipelineCompleted = true;
            });
            while (!pipelineCompleted)
            {
                Console.Clear();
                Console.WriteLine("Pipeline running ...");
                DrawPipelineState(DateTime.Now - startTime, currStatus);
                Thread.Sleep(1000);
            }
            Console.Clear();
            DrawPipelineState(DateTime.Now - startTime, currStatus);
            Console.WriteLine("==== Monitor Completed ====");
        }
        catch (Exception ex)
        {
            CommonsService.DisplayErrorMessage("Pipeline error", ex, logger: _logger);
        }
    }
    private void DisposePipeline()
    {
        _azureService.DisposePipeline();
        Console.WriteLine("Pipeline disposed successfully!");
    }
}
