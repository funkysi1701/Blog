using ImpSoft.OctopusEnergy.Api;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using WebBlog.Data.Context;
using WebBlog.Data.Services;

namespace WebBlog
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("BlogClient", client =>
            {
                client.BaseAddress = new Uri(Configuration.GetValue<string>("DEVTOURL"));
            });

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));
            services.AddControllersWithViews(options =>
            {
            }).AddMicrosoftIdentityUI();
            services.AddRazorPages().AddMicrosoftIdentityUI();
            services.AddServerSideBlazor()
                .AddCircuitOptions(opt => { opt.DetailedErrors = true; });

            services.AddSingleton<BlogService>();
            services.AddScoped<PowerService>();
            services.AddScoped<MetricService>();
            services.AddScoped<TwitterService>();
            services.AddScoped<GithubService>();
            services.AddScoped<DevToService>();

            services.AddHttpClient<IOctopusEnergyClient, OctopusEnergyClient>()
                .ConfigurePrimaryHttpMessageHandler(h => new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.All
                });

            services.AddDbContext<MetricsContext>
                (options => options.UseCosmos(
                    Configuration.GetValue<string>("CosmosDBURI"),
                    Configuration.GetValue<string>("CosmosDBKey"),
                    databaseName: "Metrics"));

            services.AddHttpContextAccessor();

            services.AddApplicationInsightsTelemetry(Configuration.GetSection("ApplicationInsights").GetValue<string>("InstrumentationKey"));

            services.AddSingleton<AppVersionInfo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.Use(
                next =>
                {
                    return async context =>
                    {
                        context.Response.OnStarting(
                            () =>
                            {
                                if (!context.Response.Headers.Keys.Contains("Report-To"))
                                {
                                    context.Response.Headers.Add("Report-To", "{\"group\":\"default\",\"max_age\":31536000,\"endpoints\":[{\"url\":\"https://eurosafeuk.report-uri.com/a/d/g\"}],\"include_subdomains\":true}");
                                }

                                context.Response.Headers.Add("NEL", "{\"report_to\":\"default\",\"max_age\":31536000,\"include_subdomains\":true, \"success_fraction\": 0}");
                                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                                context.Response.Headers.Add("Referrer-Policy", "no-referrer-when-downgrade");
                                context.Response.Headers.Add("Permissions-Policy", "microphone=()");
                                context.Response.Headers.Remove("Server");
                                context.Response.Headers.Remove("X-Powered-By");
                                context.Response.Headers.Remove("X-AspNet-Version");
                                return Task.CompletedTask;
                            });

                        await next(context);
                    };
                });
            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
            app.UseSitemapMiddleware();
        }
    }
}
