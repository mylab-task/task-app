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
            var ctx = new TaskIterationContext(
                System.Diagnostics.Activity.Current?.TraceId.ToHexString(),
                DateTime.Now
            );

            Exception iterationError = null;
            

            try
            {
                StatusService.LogicStarted();
                Logger?
                    .Action("Task logic has started")
                    .Write();

                await TaskLogic.PerformAsync(ctx, cancellationToken);

                StatusService.LogicCompleted();
                Logger?
                    .Action("Task logic has completed")
                    .Write();
            }
            catch (Exception e)
            {
                iterationError = e;

                StatusService.LogicError(e);
                Logger?
                    .Error("Task logic has fail", e)
                    .Write();
            }

            if (ProtocolWriter != null)
            {    
                try
                {
                    await ProtocolWriter.WriteAsync(ctx, DateTime.Now - ctx.StartAt, iterationError);
                }
                catch (Exception e)
                {
                    Logger?
                        .Error("Protocol writing error", e)
                        .Write();
                }
            }
        }
    }
}