using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.StatusProvider;
using Newtonsoft.Json;
using TestServer;
using Xunit;
using Xunit.Abstractions;
using TaskStatus = MyLab.StatusProvider.TaskStatus;

namespace FuncTests
{
    public class InitialTaskApplicationBehavior : IClassFixture<SuccessTestApp>
    {
        private readonly SuccessTestApp _clientFactory;
        private readonly ITestOutputHelper _output;

        public InitialTaskApplicationBehavior(SuccessTestApp clientFactory, ITestOutputHelper output)
        {
            _clientFactory = clientFactory;
            _output = output;
        }

        [Fact]
        public async Task ShouldProvideInitialAppStatus()
        {
            //Arrange
            var cl = _clientFactory.CreateClient();

            //Act
            var statusResp = await cl.GetAsync("/status");
            statusResp.EnsureSuccessStatusCode();

            var statusStr = await statusResp.Content.ReadAsStringAsync();

            _output.WriteLine(statusStr);

           var status = JsonConvert.DeserializeObject<ApplicationStatus>(statusStr);

            //Assert
            Assert.NotNull(status);
            Assert.False(status.Task.Processing);
            Assert.Null(status.Task.LastTimeDuration);
            Assert.Null(status.Task.LastTimeError);
            Assert.Null(status.Task.LastTimeStart);
        }

        [Fact]
        public async Task ShouldProvideInitialTaskStatus()
        {
            //Arrange
            var cl = _clientFactory.CreateClient();

            //Act
            var statusResp = await cl.GetAsync("/processing");
            statusResp.EnsureSuccessStatusCode();

            var statusStr = await statusResp.Content.ReadAsStringAsync();

            _output.WriteLine(statusStr);

            var status = JsonConvert.DeserializeObject<TaskStatus>(statusStr);

            //Assert
            Assert.NotNull(status);
            Assert.False(status.Processing);
            Assert.Null(status.LastTimeDuration);
            Assert.Null(status.LastTimeError);
            Assert.Null(status.LastTimeStart);
        }
    }
}
