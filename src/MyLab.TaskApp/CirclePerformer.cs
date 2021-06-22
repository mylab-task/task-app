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
        private readonly ITaskLogic _logic;
        private readonly ITaskStatusService _statusService;
        private readonly IDslLogger _log;
        private readonly TimeSpan _period;

        public CirclePerformer(
            ITaskLogic logic, 
            IOptions<TaskOptions> options,
            ITaskStatusService statusService = null, 
            ILogger<CirclePerformer> logger = null)
        {
            _logic = logic;
            _statusService = statusService;
            _log = logger.Dsl();

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
                try
                {
                    _statusService.LogicStarted();
                    await _logic.Perform(stoppingToken);
                    _statusService.LogicCompleted();
                }
                catch (Exception e)
                {
                    _statusService.LogicError(e);
                    _log?.Error("Error when perform task logic", e).Write();
                }

                await Task.Delay(_period, stoppingToken);
            }
        }
    }
}
