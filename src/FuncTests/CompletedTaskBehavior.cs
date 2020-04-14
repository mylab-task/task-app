using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.StatusProvider;
using Newtonsoft.Json;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests
{
    public class CompletedTaskBehavior: IClassFixture<SuccessTestApp>
    {
        private readonly SuccessTestApp _clientFactory;
        private readonly ITestOutputHelper _output;

        public CompletedTaskBehavior(SuccessTestApp clientFactory, ITestOutputHelper output)
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
            await Task.Delay(200);

            //Act
            var statusResp = await cl.GetAsync("/status");
            statusResp.EnsureSuccessStatusCode();

            var statusStr = await statusResp.Content.ReadAsStringAsync();

            _output.WriteLine(statusStr);

            var status = JsonConvert.DeserializeObject<ApplicationStatus>(statusStr);

            //Assert
            Assert.NotNull(status);
            Assert.False(status.Task.Processing);
            Assert.NotNull(status.Task.LastTimeDuration);
            Assert.True(status.Task.LastTimeDuration.Value.TotalMilliseconds >= 200 && status.Task.LastTimeDuration.Value.TotalMilliseconds < 250);
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
            await Task.Delay(200);

            //Act
            var statusResp = await cl.GetAsync("/processing");
            statusResp.EnsureSuccessStatusCode();

            var statusStr = await statusResp.Content.ReadAsStringAsync();

            _output.WriteLine(statusStr);

            var status = JsonConvert.DeserializeObject<MyLab.StatusProvider.TaskStatus>(statusStr);

            //Assert
            Assert.NotNull(status);
            Assert.False(status.Processing);
            Assert.NotNull(status.LastTimeDuration);
            Assert.True(status.LastTimeDuration.Value.TotalMilliseconds >= 200 && status.LastTimeDuration.Value.TotalMilliseconds < 250);
            Assert.Null(status.LastTimeError);
            Assert.NotNull(status.LastTimeStart);
            Assert.True(status.LastTimeStart.Value.AddSeconds(1) > DateTime.Now);
        }
    }
}
