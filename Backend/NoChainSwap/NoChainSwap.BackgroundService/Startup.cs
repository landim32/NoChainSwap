using NoChainSwap.Application;
using NoChainSwapBackgroundService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NoChainSwap.BackgroundService
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
            var config = new ConfigurationParam
            {
                ConnectionString = Configuration.GetConnectionString("NoChainSwapContext"),
                WalletStxApi = Configuration.GetSection("Stacks:WalletApi").Value,
                StacksApi = Configuration.GetSection("Stacks:StacksApi").Value
            };
            Initializer.Configure(services, config, false);
            services.AddHostedService<Service>();
            //services.AddHostedService<ServiceDaily>();
            services.AddTransient(typeof(ScheduleTask), typeof(ScheduleTask));
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
        }
    }
}
