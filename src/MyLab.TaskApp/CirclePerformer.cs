using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.Log.Dsl;

namespace MyLab.TaskApp
{
    class CirclePerformer : BackgroundService
    {
        private readonly TimeSpan _period;
        private readonly TaskLogicPerformer _logicPerformer;

        public CirclePerformer(
            ITaskLogic logic, 
            IOptions<TaskOptions> options,
            ITaskStatusService statusService = null, 
            ILogger<CirclePerformer> logger = null)
        {

            _logicPerformer = new TaskLogicPerformer(logic, statusService)
            {
                Logger = logger?.Dsl()
            };

            if (options.Value == null || options.Value.IdlePeriod == default)
                _period = TimeSpan.FromSeconds(1);
            else
            {
                _period = options.Value.IdlePeriod;
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
