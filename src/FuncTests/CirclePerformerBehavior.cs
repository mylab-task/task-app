using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyLab.TaskApp;
using MyLab.TaskApp.IterationContext;
using Xunit;
using Xunit.Abstractions;
using MyLab.Log.XUnit;

namespace FuncTests
{
    public class CirclePerformerBehavior
    {
        private readonly ITestOutputHelper _output;

        public CirclePerformerBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(nameof(GetPerformingTestCases))]
        public async Task ShouldPerformLogic(Exception logicException)
        {
            //Arrange
            var logic = new TestTaskLogic(logicException);

            var serviceHost = new HostBuilder()
                .ConfigureServices((context, sc) => sc
                    .AddTaskLogic(logic)
                    .AddTaskCirclePerformer()
                    .Configure<TaskOptions>(to => to.IdlePeriod = TimeSpan.FromMilliseconds(100))
                    .AddLogging(b => b.AddXUnit(_output))
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

            public Task PerformAsync(TaskIterationContext iterationContext, CancellationToken cancellationToken)
            {
                InvokeCount = InvokeCount + 1;
                if (_needThrow != null)
                    throw _needThrow;

                return Task.CompletedTask;
            }
        }
    }
}
