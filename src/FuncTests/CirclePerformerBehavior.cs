using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyLab.TaskApp;
using Xunit;

namespace FuncTests
{
    public class CirclePerformerBehavior
    {
        [Theory]
        [MemberData(nameof(GetPerformingTestCases))]
        public async Task ShouldPerformLogic(Exception logicException)
        {
            //Arrange
            var logic = new TestTaskLogic(logicException);

            var serviceHost = new HostBuilder()
                .ConfigureServices((context, sc) => sc
                    .AddTaskLogic<TestTaskLogic>(logic)
                    .AddTaskCirclePerformer(context.Configuration)
                    .Configure<TaskOptions>(to => to.IdlePeriod = TimeSpan.FromMilliseconds(100))
                )
                .Build();

            var ctSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));

           //Act
            await serviceHost.RunAsync(ctSource.Token);

            //Assert
            Assert.True(logic.InvokeCount > 1);
        }

        public static object[][] GetPerformingTestCases()
        {
            return new []
            {
                new object[] {null},
                new object[] {new Exception()}
            };
        }

        class TestTaskLogic : ITaskLogic
        {
            private readonly Exception _needThrow;
            public int InvokeCount { get; private set; }

            public TestTaskLogic(Exception needThrow = null)
            {
                _needThrow = needThrow;
            }

            public Task Perform(CancellationToken cancellationToken)
            {
                InvokeCount = InvokeCount + 1;
                if (_needThrow != null)
                    throw _needThrow;

                return Task.CompletedTask;
            }
        }
    }
}
