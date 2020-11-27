using System;
using System.Threading;
using System.Threading.Tasks;
using MyLab.TaskApp;

namespace TestServer
{
    public class SuccessTaskLogic : ITaskLogic
    {
        public Task Perform(CancellationToken cancellationToken)
        {
            return Task.Delay(200, cancellationToken);
        }
    }

    public class FailTaskLogic : ITaskLogic
    {
        public Task Perform(CancellationToken cancellationToken)
        {
            throw new Exception("foo");
        }
    }
}