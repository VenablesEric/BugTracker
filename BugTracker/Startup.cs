
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Threading.Tasks;

namespace BugTracker
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential 
                // cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                // requires using Microsoft.AspNetCore.Http;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddMvc(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Home/AccessDenied");
            });
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

                endpoints.MapGet("/Identity/Account/Manage", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));
                endpoints.MapPost("/Identity/Account/Manage", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));

                endpoints.MapGet("/Identity/Account/Manage/Email", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));
                endpoints.MapPost("/Identity/Account/Manage/Email", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));

                endpoints.MapGet("/Identity/Account/Manage/ChangePassword", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));
                endpoints.MapPost("/Identity/Account/Manage/ChangePassword", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));

                endpoints.MapGet("/Identity/Account/Manage/TwoFactorAuthentication", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));
                endpoints.MapPost("/Identity/Account/Manage/TwoFactorAuthentication", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));

                endpoints.MapGet("/Identity/Account/Manage/PersonalData", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));
                endpoints.MapPost("/Identity/Account/Manage/PersonalData", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));

                endpoints.MapGet("/Identity/Account/ForgotPassword", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));
                endpoints.MapPost("/Identity/Account/ForgotPassword", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));

                endpoints.MapGet("/Identity/Account/ForgotPasswordConfirmation", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));
                endpoints.MapPost("/Identity/Account/ForgotPasswordConfirmation", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));

                endpoints.MapGet("/Identity/Account/Manage/DeletePersonalData", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));
                endpoints.MapPost("/Identity/Account/Manage/DeletePersonalData", context => Task.Factory.StartNew(() => context.Response.Redirect("/Home/Index", true, true)));

                endpoints.MapGet("/Home/Index", context => Task.Factory.StartNew(() => context.Response.Redirect("/Projects/Index", true, true)));
                endpoints.MapPost("/Home/Index", context => Task.Factory.StartNew(() => context.Response.Redirect("/Projects/Index", true, true)));

                endpoints.MapGet("", context => Task.Factory.StartNew(() => context.Response.Redirect("/Projects/Index", true, true)));
                endpoints.MapPost("", context => Task.Factory.StartNew(() => context.Response.Redirect("/Projects/Index", true, true)));

                endpoints.MapGet("/Home", context => Task.Factory.StartNew(() => context.Response.Redirect("/Projects", true, true)));
                endpoints.MapPost("/Home", context => Task.Factory.StartNew(() => context.Response.Redirect("/Projects", true, true)));
            });
        }
    }
}
