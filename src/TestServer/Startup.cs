using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyLab.StatusProvider;
using MyLab.TaskApp;
using Newtonsoft.Json;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace TestServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services
                .AddTaskLogic(new SuccessTaskLogic())
                .AddAppStatusProviding();

            var otlpConfig = Configuration.GetSection("Otlp");

            var otlpServiceName = string.IsNullOrEmpty(otlpConfig["serviceName"])
                ? Assembly.GetEntryAssembly()?.GetName().Name ?? "[not-specified]"
                : otlpConfig["serviceName"];

            services.AddOpenTelemetry().WithTracing((builder) =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddEnvironmentVariableDetector()
                        .AddTelemetrySdk()
                        .AddService(otlpServiceName)
                    );

                if (otlpConfig.Exists())
                {
                    builder
                        .AddOtlpExporter(opt =>
                        {
                            opt.Endpoint = new Uri(otlpConfig["endpoint"]);
                            opt.Protocol = (OtlpExportProtocol)Enum.Parse(typeof(OtlpExportProtocol),
                                otlpConfig["protocol"]);
                        });
                }
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                })
                .UseTaskApi()
                .UseStatusApi(serializerSettings:new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.All
                });
        }
    }
}
