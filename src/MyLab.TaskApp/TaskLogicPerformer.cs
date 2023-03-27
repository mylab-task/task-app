using System;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Log.Dsl;
using MyLab.TaskApp.IterationContext;
using MyLab.TaskApp.Protocol;

namespace MyLab.TaskApp
{
    class TaskLogicPerformer
    {
        public ITaskLogic TaskLogic { get; }
        public ITaskStatusService StatusService { get; }
        public IDslLogger Logger { get; set; }
        public IProtocolWriter ProtocolWriter{ get; set; }

        public TaskLogicPerformer(ITaskLogic taskLogic, ITaskStatusService statusService)
        {
            TaskLogic = taskLogic ?? throw new ArgumentNullException(nameof(taskLogic));
            StatusService = statusService ?? throw new ArgumentNullException(nameof(statusService));
        }

        public void PerformLogicParallel(CancellationToken cancellationToken)
        {
            Task.Run(() => PerformLogicAsync(cancellationToken), cancellationToken);
        }

        public async Task PerformLogicAsync(CancellationToken cancellationToken)
        {
            try
            {
                StatusService.LogicStarted();
                Logger?
                    .Action("Task logic has started")
                    .Write();

                var ctx = new TaskIterationContext(
                    System.Diagnostics.Activity.Current?.TraceId.ToHexString(),
                    DateTime.Now
                );

                await TaskLogic.PerformAsync(ctx, cancellationToken);

                if (ProtocolWriter != null)
                {
                    await ProtocolWriter.WriteAsync(ctx);
                }

                StatusService.LogicCompleted();
                Logger?
                    .Action("Task logic has completed")
                    .Write();
            }
            catch (Exception e)
            {
                StatusService.LogicError(e);
                Logger?
                    .Error("Task logic has fail", e)
                    .Write();
            }
        }
    }
}