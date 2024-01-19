namespace Services.Interfaces;

public interface IDataFactoryService
{
	IDataFactoryService TriggerPipeline();
	IAsyncEnumerable<string> MonitorPipeline();
	void DisposePipeline();
	bool IsPipelineConfigured();
}