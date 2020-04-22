using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MyLab.StatusProvider;

namespace MyLab.TaskApp
{
    /// <summary>
    /// Integrates task-application logic into application
    /// </summary>
    public static class TaskLogicIntegration
    {
        /// <summary>
        /// Integrate task logic into DI container as singleton
        /// </summary>
        public static IServiceCollection AddTaskLogic<T>(this IServiceCollection srv, T logic)
            where T : class, ITaskLogic
        {
            return srv
                .AddSingleton<ITaskStatusService, DefaultTaskStatusService>()
                .AddSingleton<ITaskLogic>(logic);
        }

        /// <summary>
        /// Integrate task logic into DI container with scoped lifetime
        /// </summary>
        public static IServiceCollection AddTaskLogic<T>(this IServiceCollection srv)
            where T : class, ITaskLogic
        {
            return srv
                .AddSingleton<ITaskStatusService, DefaultTaskStatusService>()
                .AddSingleton<ITaskLogic, T>();
        }

        /// <summary>
        /// Integrates url handling
        /// </summary>
        public static IApplicationBuilder UseTaskApi(this IApplicationBuilder app, string path = "/processing")
        {
            app.MapWhen(ctx =>
                ctx.Request.Path == path && ctx.Request.Method == "GET",
                appB =>
                {
                    appB.Run(async context => await TaskLogicApiHandler.GetStatus(app, context));
                });

            app.MapWhen(ctx =>
                ctx.Request.Path == path && ctx.Request.Method == "POST",
                appB =>
                {
                    appB.Run(async context => await TaskLogicApiHandler.StartLogic(app, context));
                });
            return app;
        }
    }
}
