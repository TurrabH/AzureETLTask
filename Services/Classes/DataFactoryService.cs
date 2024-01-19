using Azure.Identity;
using Domains.ConfigModels;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Rest;
using Services.Interfaces;

namespace Services.Classes;

public class DataFactoryService : IDataFactoryService
{
	private readonly AzureClient _azureClient;
	private readonly DataFactoryClient _dataFactoryClient;
	private readonly DefaultAzureCredential _azureCredentials;
	private readonly string[] _pipelineRunningStatuses = new string[] { "InProgress", "Queued" };
	private DataFactoryManagementClient? _dataFactoryManagementClient;
	private string? pipelineId;
	public DataFactoryService(AzureClient azureClient)
	{
		_azureClient = azureClient ?? throw new ArgumentNullException(nameof(azureClient));
		_dataFactoryClient = _azureClient.DataFactoryClient;
		_azureCredentials = new DefaultAzureCredential();
	}
	public IDataFactoryService TriggerPipeline()
	{
		pipelineId = TriggerPipelineRunAsync().RunId;
		return this;
	}

	public async IAsyncEnumerable<string> MonitorPipeline()
	{
		if (pipelineId != null)
		{
			string? pipelineStatus;
			while (true)
			{
				var pipelineRun = GetDataFactoryClient().PipelineRuns.Get(_dataFactoryClient.ResourceGroup, _dataFactoryClient.DataFactory, pipelineId);
				yield return pipelineStatus = pipelineRun.Status;
				if (_pipelineRunningStatuses.Contains(pipelineStatus)) await Task.Delay(5000);
				else yield break;
			}
		}
	}

	public void DisposePipeline() => pipelineId = null;

	public bool IsPipelineConfigured() => pipelineId != null;

	private CreateRunResponse TriggerPipelineRunAsync()
		=> GetDataFactoryClient()
		.Pipelines
		.CreateRunWithHttpMessagesAsync(
			_dataFactoryClient.ResourceGroup,
			_dataFactoryClient.DataFactory,
			_dataFactoryClient.Pipeline)
		.Result
		.Body;

	private DataFactoryManagementClient InitDataFactoryManagementClient()
	{
		var accessToken = _azureCredentials.GetToken(new Azure.Core.TokenRequestContext(_azureClient.Scopes)).Token;
		var credentials = new TokenCredentials(accessToken);
		return new DataFactoryManagementClient(credentials)
		{
			SubscriptionId = _azureClient.SubscriptionId
		};
	}

	private DataFactoryManagementClient GetDataFactoryClient()
	{
		if (_dataFactoryManagementClient == null)
			_dataFactoryManagementClient = InitDataFactoryManagementClient();
		return _dataFactoryManagementClient;
	}
}
