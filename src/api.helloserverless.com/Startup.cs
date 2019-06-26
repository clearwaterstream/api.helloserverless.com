using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace api.helloserverless.com
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        readonly Logger logger = null;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;

            var logFactory = LogManager.LoadConfiguration("NLog.config");
            logger = logFactory.GetCurrentClassLogger();

            logger.Info("app is booting");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var appLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();

            appLifetime.ApplicationStopping.Register(OnShuttingDown);

            // global hanlder for any uncaught exceptions
            app.UseExceptionHandler(new ExceptionHandlerOptions() { ExceptionHandler = GlobalErrorHandler.ExceptionHandlerDelegate });

            app.UseMvc(ConfigureRoutes);
        }

        protected virtual void ConfigureRoutes(IRouteBuilder routes)
        {
            routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
        }

        void OnShuttingDown()
        {
            logger.Info("app is shutting down");

            LogManager.Flush(3000); // flush any remaining messages
        }
    }
}
