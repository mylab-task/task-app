using System;
using System.Threading.Tasks;
using MyLab.LogDsl;
using MyLab.StatusProvider;

namespace MyLab.TaskApp
{
    class TaskLogicPerformer
    {
        public ITaskLogic TaskLogic { get; }
        public IAppStatusService StatusService { get; }

        public DslLogger Logger { get; set; }

        public TaskLogicPerformer(ITaskLogic taskLogic, IAppStatusService statusService)
        {
            TaskLogic = taskLogic ?? throw new ArgumentNullException(nameof(taskLogic));
            StatusService = statusService ?? throw new ArgumentNullException(nameof(statusService));
        }

        public void PerformLogicParallel()
        {
            Task.Run(PerformLogicAsync);
        }

        async Task PerformLogicAsync()
        {
            try
            {
                StatusService.TaskLogicStarted();
                Logger?.Act("Task logic has started");
                await TaskLogic.Perform();
                StatusService.TaskLogicCompleted();
                Logger?.Act("Task logic has completed");
            }
            catch (Exception e)
            {
                StatusService.TaskLogicError(e);
                Logger?.Error("Task logic has fail", e);
            }
        }
    }
}