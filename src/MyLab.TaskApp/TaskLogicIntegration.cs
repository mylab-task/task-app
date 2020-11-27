using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyLab.TaskApp
{
    /// <summary>
    /// Integrates task-application logic into application
    /// </summary>
    public static class TaskLogicIntegration
    {
        private const string DefaultConfigSectionName = "Task";

        /// <summary>
        /// Integrate task logic into DI container as singleton
        /// </summary>
        public static IServiceCollection AddTaskLogic<T>(this IServiceCollection srv, T logic)
            where T : class, ITaskLogic
        {
            if (srv == null) throw new ArgumentNullException(nameof(srv));
            if (logic == null) throw new ArgumentNullException(nameof(logic));

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
            if (srv == null) throw new ArgumentNullException(nameof(srv));

            return srv
                .AddSingleton<ITaskStatusService, DefaultTaskStatusService>()
                .AddSingleton<ITaskLogic, T>();
        }

        /// <summary>
        /// Add circle logic performer
        /// </summary>
        public static IServiceCollection AddTaskCirclePerformer(this IServiceCollection srv, IConfiguration config, string sectionName = DefaultConfigSectionName)
        {
            if (srv == null) throw new ArgumentNullException(nameof(srv));
            if (config == null) throw new ArgumentNullException(nameof(config));

            return srv
                .Configure<TaskOptions>(config.GetSection(sectionName))
                .AddHostedService<CirclePerformer>();
        }


        /// <summary>
        /// Integrates url handling
        /// </summary>
        public static IApplicationBuilder UseTaskApi(this IApplicationBuilder app, string path = "/processing")
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (path == null) throw new ArgumentNullException(nameof(path));

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
