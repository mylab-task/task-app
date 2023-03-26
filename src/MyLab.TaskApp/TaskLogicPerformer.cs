using System;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Log.Dsl;
using MyLab.ProtocolStorage.Client;
using MyLab.ProtocolStorage.Client.Models;

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
                
                var iterationResult = await TaskLogic.Perform(cancellationToken);

                if (ProtocolWriter != null)
                {
                    await ProtocolWriter.WriteAsync(iterationResult);
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