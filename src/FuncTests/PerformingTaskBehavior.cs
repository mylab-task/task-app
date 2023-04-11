using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyLab.ProtocolStorage.Client;
using MyLab.ProtocolStorage.Client.Models;
using MyLab.StatusProvider;
using MyLab.TaskApp;
using MyLab.TaskApp.IterationContext;
using MyLab.TaskApp.Protocol;
using Newtonsoft.Json;
using TestServer;
using Xunit;
using Xunit.Abstractions;

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

            var appStatus = JsonConvert.DeserializeObject<ApplicationStatus>(statusStr, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            var status = appStatus?.GetSubStatus<TaskAppStatus>();

            //Assert
            Assert.NotNull(status);
            Assert.True(status.Processing);
            Assert.Null(status.LastTimeDuration);
            Assert.Null(status.LastTimeError);
            Assert.NotNull(status.LastTimeStart);
            Assert.True(status.LastTimeStart.Value.AddSeconds(1) > DateTime.Now);
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

            var status = JsonConvert.DeserializeObject<TaskAppStatus>(statusStr);

            //Assert
            Assert.NotNull(status);
            Assert.True(status.Processing);
            Assert.Null(status.LastTimeDuration);
            Assert.Null(status.LastTimeError);
            Assert.NotNull(status.LastTimeStart);
            Assert.True(status.LastTimeStart.Value.AddSeconds(1) > DateTime.Now);
        }

        [Fact]
        public async Task ShouldReturnSpecialCodeIfLogicAlreadyPerforming()
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

        [Fact]
        public async Task ShouldWriteProtocol()
        {
            //Arrange
            string capturedProtocolId = null;
            ProtocolEvent capturedProtocolEvent = null;
            TaskIterationProtocolEvent capturedTaskProtocolEvent = null;

            var protocolMock = new Moq.Mock<IProtocolApiV1>();

            protocolMock
                .Setup(p => p.PostEventAsync(It.IsAny<string>(), It.IsAny<ProtocolEvent>()))
                .Returns<string, ProtocolEvent>((pId, pEvent) =>
                {
                    capturedProtocolEvent = pEvent;
                    capturedTaskProtocolEvent = pEvent as TaskIterationProtocolEvent;
                    capturedProtocolId = pId;
                    return Task.CompletedTask;
                });

            var cl = _clientFactory.WithWebHostBuilder(
                b =>
                    b.ConfigureServices(s => s.AddSingleton(protocolMock.Object))
                )
                .CreateClient();

            //Act
            var statusResp = await cl.PostAsync("/processing", null);

            await Task.Delay(500);

            if (capturedTaskProtocolEvent != null)
                _output.WriteLine(JsonConvert.SerializeObject(capturedTaskProtocolEvent, Formatting.Indented));

            //Assert
            Assert.Equal(HttpStatusCode.OK, statusResp.StatusCode);
            Assert.Equal(ProtocolEventConstants.DefaultProtocolId, capturedProtocolId);
            Assert.NotNull(capturedProtocolEvent);
            Assert.Equal(ProtocolEventConstants.Type, capturedProtocolEvent.Type);
            Assert.NotNull(capturedTaskProtocolEvent);
            Assert.Equal(IterationWorkload.Useful, capturedTaskProtocolEvent.Workload);
            Assert.Equal("bar", capturedTaskProtocolEvent.Id);
            Assert.Equal("foo", capturedTaskProtocolEvent.Subject);
            Assert.Equal(TaskKicker.Api, capturedTaskProtocolEvent.Kicker);
        }
    }
}
