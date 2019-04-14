using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Modix.Auth;
using Modix.Configuration;
using Modix.Data;
using Modix.Data.Models.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Modix
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            Log.Information("Configuration loaded. ASP.NET Startup is a go.");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ModixConfig>(_configuration);

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"dataprotection"));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/api/unauthorized";
                    //options.LogoutPath = "/logout";
                    options.ExpireTimeSpan = new TimeSpan(7, 0, 0, 0);

                })
                .AddModixAuth(_configuration);

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            services.AddResponseCompression();

            services.AddTransient<IConfigureOptions<StaticFileOptions>, StaticFilesConfiguration>();
            services.AddTransient<IStartupFilter, ModixConfigValidator>();

            services.AddDbContext<ModixContext>(options =>
            {
                options.UseNpgsql(_configuration.GetValue<string>(nameof(ModixConfig.DbConnection)));
            });

            services
                .AddModixHttpClients()
                .AddModix();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.Converters.Add(new StringULongConverter());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == Environments.Development)
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCompression();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(builder =>
            {
                // Static redirect for invite link
                builder.Map("/invite", context =>
                {
                    // TODO: Maybe un-hardcode this?
                    context.Response.Redirect("https://aka.ms/csharp-discord");
                    return Task.CompletedTask;
                });

                builder.Map("/{non-api:regex([^(?:api/?)])}", context =>
                {
                    if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
                    {
                        context.Request.Path = "/index.html";
                    }

                    return Task.CompletedTask;
                });

                builder.MapControllers();
            });
        }
    }
}
