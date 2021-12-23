using System;
using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(BugTracker.Areas.Identity.IdentityHostingStartup))]
namespace BugTracker.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<BugTrackerDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("BugTrackerDbContextConnection")));

                services.AddDefaultIdentity<BugTrackerUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<BugTrackerDbContext>();

                services.Configure<SecurityStampValidatorOptions>(options =>
                {
                    // enables immediate logout, after updating security stamp.
                    options.ValidationInterval = TimeSpan.Zero;
                });
            });
        }
    }
}