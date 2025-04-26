using Konscious.Security.Cryptography;
using ProjectManager.Models.Services;
using ProjectManager.Encryption;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ProjectManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.Cookie.Name = "MyProjectCookie";
                options.Cookie.HttpOnly = true;
                //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                //options.AccessDeniedPath = "/";  Directing Authenticated users
            });

            var app = builder.Build();
            MongoManipulator.Initialize(builder.Configuration);
            Argon2Helper.Initialize(builder.Configuration);


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
