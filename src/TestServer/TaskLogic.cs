using System;
using System.Threading.Tasks;
using MyLab.TaskApp;

namespace TestServer
{
    public class SuccessTaskLogic : ITaskLogic
    {
        public Task Perform()
        {
            return Task.Delay(200);
        }
    }

    public class FailTaskLogic : ITaskLogic
    {
        public Task Perform()
        {
            throw new Exception("foo");
        }
    }
}