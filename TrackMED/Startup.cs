using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.Email;

using System.Net;

using TrackMED.Extensions;
using Microsoft.Extensions.Logging;

namespace TrackMED
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Serilog.Debugging.SelfLog.Enable(Console.WriteLine);
            /*
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets();
            }
            

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            */

            EmailConnectionInfo info = new EmailConnectionInfo()
            {
                EmailSubject = "Test Serilog",
                ToEmail = "jul_soriano@yahoo.com",            
                FromEmail = "elijahpne@gmail.com",
                MailServer = "smtp.gmail.com", // "smtp.live.com"
                NetworkCredentials = new NetworkCredential("elijahpne@gmail.com", "acts15:23GOOG"),
                Port = 587,
                
                /*
                FromEmail = "jul_soriano@hotmail.com",
                MailServer = "smtp.live.com", // "smtp.live.com"                
                NetworkCredentials = new NetworkCredential("jul_soriano@hotmail.com", "acts15:23hot"),
                Port = 465, // 465
                */
            };

            // Or This: See https://github.com/serilog/serilog/wiki/Getting-Started
            var uriMongoDB = Configuration.GetValue<string>("MongoSettings:mongoconnection");

            // http://sourcebrowser.io/Browse/serilog/serilog/src/Serilog.Sinks.Email/LoggerConfigurationEmailExtensions.cs
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.Console()
                //.WriteTo.LiterateConsole()
                //.WriteTo.RollingFile("logs\\trackmed-{Date}.txt")
                // .WriteTo.Email(info, restrictedToMinimumLevel: LogEventLevel.Warning)   // https://github.com/serilog/serilog/wiki/Configuration-Basics#overriding-per-sink
                //.WriteTo.Seq("http://localhost:5341/")
                .WriteTo.MongoDBCapped(uriMongoDB, collectionName: "logsTrackMED")  // https://github.com/serilog/serilog-sinks-mongodb
                .CreateLogger();

            Log.Warning("TrackMED_Core21_MVC is launched");
        }

        /*
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
        }
        */
        public IConfiguration Configuration { get; }
        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddRazorPages();

            // Add our Config object so it can be injected; needs "Microsoft.Extensions.Options.ConfigurationExtensions": "1.0.0"
            // See http://stackoverflow.com/questions/36893326/read-a-value-from-appsettings-json-in-1-0-0-rc1-final
            services.Configure<Settings>(Configuration.GetSection("MongoSettings"));

            services.RegisterServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            // loggerFactory.AddDebug();
            //loggerFactory.AddEmail(, LogLevel.Critical);

            // See https://www.towfeek.se/2016/06/structured-logging-with-aspnet-core-using-serilog-and-seq/
            // loggerFactory.AddSerilog();     // requires Serilog.Extensions,Logging

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            /* Migrating from .Net Core 2.2 to 3.0, See https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-3.0&tabs=visual-studio
                 or See: https://www.evernote.com/shard/s102/nl/11219721/b69c73f9-4dbe-4688-baef-1513c808e046

               Replaces useMVC
            */
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                /* Note: 
                 * There is no action in StatusController named 'Index'. 
                 * Routing probably looks at the signature rather than the action name
                      so this defaults to GetAllAsync which has the specified signature.
                 * Works in conjunction with launchsettings.json. THIS JSON MUST BE SPECIFIED
                */
            });

            /*
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            */
        }
    }
}
