using System;
using System.Threading;
using System.Threading.Tasks;
using MyLab.TaskApp;
using MyLab.TaskApp.IterationContext;

namespace TestServer
{
    public class SuccessTaskLogic : ITaskLogic
    {
        public Task PerformAsync(TaskIterationContext iterationContext, CancellationToken cancellationToken)
        {
            return Task.Delay(200, cancellationToken);
        }
    }

    public class FailTaskLogic : ITaskLogic
    {
        public Task PerformAsync(TaskIterationContext iterationContext, CancellationToken cancellationToken)
        {
            throw new Exception("foo");
        }
    }
}