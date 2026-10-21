using Amazon;
using Amazon.S3;
using AspNetCore.DataProtection.Aws.S3;
using Librerias.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Librerias
{
    public class Startup {
        public IConfiguration Configuration { get; } 
 


        public Startup(IConfiguration configuration ) {
            Configuration = configuration;
          
        }

     
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext <LibreriasContext>(options => options.UseMySQL(Configuration.GetConnectionString("db")));
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();


            services.Configure<CookiePolicyOptions>(options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                
            });
            
            
            services.AddSession(options => {
                options.Cookie.Name = ".Soft.Session";
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.IsEssential = true;
            });


            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizePage("/Index");
                options.Conventions.AuthorizeFolder("/");
                options.Conventions.AllowAnonymousToPage("/Login");
            });


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
                options.LoginPath = new PathString("/Login");
                options.ExpireTimeSpan = TimeSpan.FromHours(10);
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.IsEssential = true;
            });


            services.Configure<IdentityOptions>(options => {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            services.Configure<RequestLocalizationOptions>(options => {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US") };
                options.RequestCultureProviders.Clear();
            });

            services.AddAWSService<IAmazonS3>();
            services.AddDataProtection().PersistKeysToAwsS3(
                 new AmazonS3Client(RegionEndpoint.SAEast1),
                 new S3XmlRepositoryConfig("multidealerfiles")
                 {
                     MaxS3QueryConcurrency = 100,
                     KeyPrefix = "DataProtectionKeys/Libreria/",
                 });


             


            CultureInfo cultura = new CultureInfo("en-US");
            cultura.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
            cultura.DateTimeFormat.FullDateTimePattern = "dd/MM/yyyy HH:mm";
            cultura.DateTimeFormat.LongDatePattern = "dddd, d' de 'MMMM' de 'yyyy";
            cultura.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            cultura.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            cultura.DateTimeFormat.ShortTimePattern = "HH:mm";
            CultureInfo.CurrentCulture = cultura;
                       
          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseSession();
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {

                app.UseExceptionHandler("/Error");
                app.UseHsts();

            }

            app.UseCors(
              options => options
                  .WithOrigins("*")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .SetPreflightMaxAge(TimeSpan.FromSeconds(2520))
              );


            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseRequestLocalization();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

        }
    }
}
