using System;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Log.Dsl;

namespace MyLab.TaskApp
{
    class TaskLogicPerformer
    {
        public ITaskLogic TaskLogic { get; }
        public ITaskStatusService StatusService { get; }

        public IDslLogger Logger { get; set; }

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
                Logger?.Action("Task logic has started");
                await TaskLogic.Perform(CancellationToken.None);
                StatusService.LogicCompleted();
                Logger?.Action("Task logic has completed");
            }
            catch (Exception e)
            {
                StatusService.LogicError(e);
                Logger?.Error("Task logic has fail", e);
            }
        }
    }
}