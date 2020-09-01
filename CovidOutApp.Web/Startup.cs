using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using CovidOutApp.Web.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CovidOutApp.Web.Models;
using CovidOutApp.Web.Repositories;
using CovidOutApp.Web.ServiceLayer;
using System.IO;
using CovidOutApp.Web.Messaging;

namespace CovidOutApp.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
    

        // This method gets called by the runtime. Use this method to add services to the container.
        
        private void CofigureRepositoriesDI(IServiceCollection services){
            services.AddTransient<IVenueRepository,VenueRepository>();
            services.AddTransient<IVenueRegistrationApplicationRepository,VenueRegistrationApplicationRepository>();
            services.AddTransient<IVenueVisitRepository, VenueVisitRepository>();
            services.AddTransient<IVenueImageRepository, VenueImageRepository>();
            services.AddTransient<IVenueRulesRepository,VenueRulesRepository>();
        }

        private void CofigureServicesDI(IServiceCollection services){
            services.AddTransient<IVenueService,VenueService>();
            services.AddTransient<IVenueRegistrationService, VenueRegistrationService>();
            services.AddTransient<IVisitorManagementService, VisitorManagementService>();
            services.AddTransient<IVenueRulesService, VenueRulesService>();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")
                    ));
            
            
            services.AddIdentity<ApplicationUser, IdentityRole>
                (options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();
                
            CofigureRepositoriesDI(services);
            CofigureServicesDI(services);
           
            services.AddTransient<IExtendedEmailSender, StandardEmailMessager>();
            services.Configure<SMTP>(Configuration.GetSection("SMTP"));

            services.AddControllersWithViews();
            services.AddRazorPages();
        
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                Globals.HostingEnvironment  = "DEV";
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                Globals.HostingEnvironment  = "PROD";
            }

            Globals.FILE_UPLOAD_DIR = Path.Combine(env.WebRootPath,"VenueFiles");

            if (!Directory.Exists(Globals.FILE_UPLOAD_DIR))
                Directory.CreateDirectory(Globals.FILE_UPLOAD_DIR);

            Globals.QRCODE_DIR = Path.Combine(env.WebRootPath,"QRCODES");

            Globals.Emails_DIR = Path.Combine(env.WebRootPath,"Emails");

             if (!Directory.Exists(Globals.QRCODE_DIR))
                Directory.CreateDirectory(Globals.QRCODE_DIR);

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
            });

            Helpers.IdentityRoleHelpers.InitializeIdentityData(app.ApplicationServices);
        }
    }
}
