using System;
using System.Net;
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
    public class PerformingTaskBehavior : IClassFixture<SuccessTestApp>
    {
        private readonly SuccessTestApp _clientFactory;
        private readonly ITestOutputHelper _output;

        public PerformingTaskBehavior(SuccessTestApp clientFactory, ITestOutputHelper output)
        {
            _clientFactory = clientFactory;
            _output = output;
        }

        [Fact]
        public async Task ShouldProvideInProcessAppStatus()
        {
            //Arrange
            var cl = _clientFactory.CreateClient();

            var statusResp0 = await cl.PostAsync("/processing", null);
            statusResp0.EnsureSuccessStatusCode();

            //Act
            var statusResp = await cl.GetAsync("/status");
            statusResp.EnsureSuccessStatusCode();

            var statusStr = await statusResp.Content.ReadAsStringAsync();

            _output.WriteLine(statusStr);

            var status = JsonConvert.DeserializeObject<ApplicationStatus>(statusStr);

            //Assert
            Assert.NotNull(status);
            Assert.True(status.Task.Processing);
            Assert.Null(status.Task.LastTimeDuration);
            Assert.Null(status.Task.LastTimeError);
            Assert.NotNull(status.Task.LastTimeStart);
            Assert.True(status.Task.LastTimeStart.Value.AddSeconds(1) > DateTime.Now);
        }

        [Fact]
        public async Task ShouldProvideInProcessTaskStatus()
        {
            //Arrange
            var cl = _clientFactory.CreateClient();
            var statusResp0 = await cl.PostAsync("/processing", null);
            statusResp0.EnsureSuccessStatusCode();

            //Act
            var statusResp = await cl.GetAsync("/processing");
            statusResp.EnsureSuccessStatusCode();

            var statusStr = await statusResp.Content.ReadAsStringAsync();

            _output.WriteLine(statusStr);

            var status = JsonConvert.DeserializeObject<TaskStatus>(statusStr);

            //Assert
            Assert.NotNull(status);
            Assert.True(status.Processing);
            Assert.Null(status.LastTimeDuration);
            Assert.Null(status.LastTimeError);
            Assert.NotNull(status.LastTimeStart);
            Assert.True(status.LastTimeStart.Value.AddSeconds(1) > DateTime.Now);
        }

        [Fact]
        public async Task ShouldReturnSpecialCodeIfLogicAlreadyPrforming()
        {
            //Arrange
            var cl = _clientFactory.CreateClient();
            var statusResp0 = await cl.PostAsync("/processing", null);
            statusResp0.EnsureSuccessStatusCode();

            //Act
            var statusResp = await cl.PostAsync("/processing", null);
            
            //Assert
            Assert.Equal(HttpStatusCode.AlreadyReported, statusResp.StatusCode);
        }
    }
}
