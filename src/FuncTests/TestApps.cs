using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyLab.TaskApp;
using TestServer;

namespace FuncTests
{
    public class SuccessTestApp : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(s =>
            {
                s.AddSingleton<ITaskLogic>(new SuccessTaskLogic());
            });
        }
    }

    public class FailTestApp : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(s =>
            {
                s.AddSingleton<ITaskLogic>(new FailTaskLogic());
            });
        }
    }
}
