using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.Log.Dsl;
using MyLab.ProtocolStorage.Client;
using MyLab.TaskApp.IterationContext;
using MyLab.TaskApp.Protocol;

namespace MyLab.TaskApp
{
    class CirclePerformer : BackgroundService
    {
        private readonly TimeSpan _period;
        private readonly TaskLogicPerformer _logicPerformer;

        public CirclePerformer(
            ITaskLogic logic, 
            IOptions<TaskOptions> options,
            IProtocolApiV1 protocolApi = null,
            ITaskStatusService statusService = null, 
            ILogger<CirclePerformer> logger = null)
        {
            var log = logger?.Dsl();

            var opts = options.Value;

            IProtocolWriter protocolWriter = null;

            if (protocolApi != null)
            {
                protocolWriter = new ProtocolWriter(
                    new SafeProtocolIndexerV1(protocolApi, log), 
                    opts.ProtocolId)
                {
                    TaskKicker = TaskKicker.Scheduler
                };
            }

            _logicPerformer = new TaskLogicPerformer(logic, statusService)
            {
                Logger = log,
                ProtocolWriter = protocolWriter
            };

            if (options.Value == null || options.Value.IdlePeriod == default)
                _period = TimeSpan.FromSeconds(1);
            else
            {
                _period = opts.IdlePeriod;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _logicPerformer.PerformLogicAsync(stoppingToken);
                await Task.Delay(_period, stoppingToken);
            }
        }
    }
}
