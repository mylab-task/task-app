using System;
using System.Threading.Tasks;
using MyLab.LogDsl;
using MyLab.StatusProvider;

namespace MyLab.TaskApp
{
    class TaskLogicPerformer
    {
        public ITaskLogic TaskLogic { get; }
        public ITaskStatusService StatusService { get; }

        public DslLogger Logger { get; set; }

        public TaskLogicPerformer(ITaskLogic taskLogic, ITaskStatusService statusService)
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
                StatusService.LogicStarted();
                Logger?.Act("Task logic has started");
                await TaskLogic.Perform();
                StatusService.LogicCompleted();
                Logger?.Act("Task logic has completed");
            }
            catch (Exception e)
            {
                StatusService.LogicError(e);
                Logger?.Error("Task logic has fail", e);
            }
        }
    }
}