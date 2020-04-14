using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyLab.LogDsl;
using MyLab.StatusProvider;
using Newtonsoft.Json;

namespace MyLab.TaskApp
{
    internal class TaskLogicApiHandler
    {
        public static async Task GetStatus(IApplicationBuilder app, HttpContext context)
        {
            var statusService = (IAppStatusService)app.ApplicationServices.GetService(typeof(IAppStatusService));
            if (statusService == null)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("No status found");
            }
            else
            {
                var status = statusService.GetStatus().Task;
                var statusTxt = JsonConvert.SerializeObject(status, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });

                context.Response.StatusCode = 200;
                context.Response.Headers.Append("Content-Type", "application/json");
                await context.Response.WriteAsync(statusTxt);
            }
        }

        public static async Task StartLogic(IApplicationBuilder app, HttpContext context)
        {
            var taskLogic = (ITaskLogic)app.ApplicationServices.GetService(typeof(ITaskLogic));
            if (taskLogic== null)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("No task logic found");

                return;
            }

            var statusService = (IAppStatusService)app.ApplicationServices.GetService(typeof(IAppStatusService));
            if (statusService == null)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("No status found");

                return;
            }

            if (statusService.GetStatus().Task.Processing)
            {
                context.Response.StatusCode = 208;
                await context.Response.WriteAsync("Already processing");

                return;
            }

            var loggerFactory = (ILoggerFactory)app.ApplicationServices.GetService(typeof(ILoggerFactory));
            var logger = loggerFactory.CreateLogger("TaskLogic");

            var performer = new TaskLogicPerformer(taskLogic, statusService)
            {
                Logger = logger.Dsl()
            };

            performer.PerformLogicParallel();

            context.Response.StatusCode = 200;
        
        }
    }
}