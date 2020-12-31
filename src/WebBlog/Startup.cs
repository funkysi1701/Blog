using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using WebBlog.Data;

namespace WebBlog
{
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
            services.AddRazorPages();
            services.AddServerSideBlazor()
                .AddCircuitOptions(opt => { opt.DetailedErrors = true; });
            services.AddSingleton<BlogService>();
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
                                context.Response.Headers.Add("Report-To", "{\"group\":\"default\",\"max_age\":31536000,\"endpoints\":[{\"url\":\"https://eurosafeuk.report-uri.com/a/d/g\"}],\"include_subdomains\":true}");
                                // context.Response.Headers.Add("Content-Security-Policy-Report-Only", "style-src-elem 'sha256-9Dtc3YPip6m3wKuaUzmxsvAVQQd8QSTNUuPlArqDF6Y=' 'sha256-Du/dlHp16Buuu6mT/U15IDZ7X/3tq50TG6U2KZs23ec=' 'sha256-nahUZetg4ey0dfBcMz7nhXJBQDeLBstIkzWAhY5ztO8=' 'sha256-Rkv0Ywffkru9o5fveSlf0yRwoMtNCphXeUubodsKxjM=' 'sha256-utNsGqty4v5mG9Bxxj9CqZc9H81JjDhaAgXnZXvSvbQ=' 'sha256-NhFL0YpEj1X5/pw6LO7Q+xzG0YX8IEvUN94VYLi4IBk=' 'sha256-ctIDKIxEGKoklB4MReGM5J6KTacpZcYuHjoESEUyMoc=' 'sha256-0Dagilvc7wSbdHwc/osxuxqEHwWCLQRXnuTM5hACbJU=' 'sha256-iSo+c78wcD57OfTQkJHeOs/OP+vtNTIcL7oKt5MZTp0=' 'sha256-rqoOufAU+rX++1IqYbHTcPtYgTRaXey1ROliKhTTL3A=' 'sha256-8oI7rKlhqHpq+BK3Nwb1GukkQZsGhoF4WcxdtavVt94=' 'sha256-zYWo3EE5InhbXmPUXoGW7NAYjy9KRAza43QcDVycERM=' 'sha256-HFJ74un5tKuGh781zS3qovKsh4wT1t6Yt5s2J0CJ2oI=' 'sha256-0gKTNkSpIjc9GgskBk9V1bQtj89dFuMYllZ+8Xrk4Zc=' 'sha256-5uVSZkVlTFKblKs2zqPTnMj9BrgW/KJdbWQXe/L4JRY=' 'sha256-UUDfsFG8hxVEHAb81zJi3nG8sytAXGtoewl3zuBY4MM=' 'sha256-Hcc2mDrXbYligKE+KKly+SVgqocIG3l9lwekBNfH2HA=' 'sha256-MRvT5C8bmPmProSB28SKbEzrujV2qZTdssVzIDrMBIM=' 'sha256-Z8ptOnlrXS2pTyfhjxaUm7BEayr91DRNNrBHN3h5ir0=' 'sha256-ZsuaOm4dERmsF3GHLks1mE9LUJlh245UBal2zpvjY4w=' 'sha256-Sce3L9wJ7HAs+IgBWYO0Q7RPPrkyTuvY43UnrWsqdAI=' 'sha256-EeZYnKegObibInu4Ljt76EGJ49HfpQMzV9Uu1DG3MvI=' 'sha256-viCpak/1gS1Y+sz84x4lfa1ahIfYXW6VrX/Wk8P+Cos=' 'sha256-yC0HN+PzvY7b2K/sS+rmJhBcEglZQFAnn2cmyJjacAU=' 'sha256-tfcDNkDteKuKOMI06egzrmgF+buTj5xFI/N+aBZdnqE=' 'sha256-dumsEzW6guQq8alWOMXUk336jU+lcTM7UWWqaEFOS4o=' 'sha256-xhIKZfZ5J0HK1NUU2//jyfCubtQSrpIlhVBe+KdQPBs=' 'sha256-X59iUrpNh8SyecfXKSo2Bx9cHshxJYQOJIMZNM+kOG4=' 'sha256-EEBvpkcjDtEN6lz/1+x4Sdt7wDUEhgjc410JA4wYcnI=' 'sha256-OLPD6clY8ckyh01g1Ouyy5jlu6Q2itWPFrxW7B7VWMw=' 'sha256-9DRIP2r3qlpTd62GLZ9a5M8OxixISr9mA8HKjEpUuMg=' 'sha256-47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=' 'sha256-rD4fG1yxWIdlrdhm3OYHiDCUSrMaVkEMso+bKvgnGPY=' 'sha256-xup84hFX/epNEa9D6F1ioyJt2s1HAcrLI+fRGvV6GyY=' 'sha256-dR+4nGwF1s6TmPWEZYqL2safyAADihpjGS6Y87x9bi4=' use.fontawesome.com fonts.googleapis.com 'self'; connect-src wss://projectpyramid-dev.eurosafeuk.co.uk dc.services.visualstudio.com wss://projectpyramid-test.eurosafeuk.co.uk wss://projectpyramid.eurosafeuk.co.uk 'self'; font-src fonts.gstatic.com use.fontawesome.com 'self'; script-src-elem 'sha256-y3Nu9atQik58VoeGrAhX48naIdzYO2Z2FHzH8r8rU8Y=' 'sha256-cSUfyladJFAUBc9graRxXLmgRgZH4tgs3XDtiwI436Q=' az416426.vo.msecnd.net 'self'; manifest-src 'self'; img-src 'self' data; report-uri https://eurosafeuk.report-uri.com/r/d/csp/wizard");
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
            app.UseSitemapMiddleware();
        }
    }
}