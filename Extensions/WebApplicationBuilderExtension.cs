using FinanceApp.Controllers;
using FinanceApp.Data;
using FinanceApp.Models;
using FinanceApp.Repositories;
using FinanceApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinanceApp.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        // Add Cookie Auth with option to sign out all users when server restarts.
        builder
            .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.DataProtectionProvider = new EphemeralDataProtectionProvider();
                options.LoginPath = "/UserAccount/Login";
            });
        builder.Services.AddAuthorization();

        // Adding DbContext with connectionstring from appsettings.json.
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddRazorPages();

        // Adding a cache.
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<CacheService>();

        // Adding application services.
        builder.Services.AddScoped<UserController>();
        builder.Services.AddScoped<AccountController>();
        builder.Services.AddScoped<TransactionController>();
        builder.Services.AddScoped<PasswordService>();
        builder.Services.AddScoped<IUserRepositorySQL, UserRepositorySQL>();
        builder.Services.AddScoped<IAccountRepositorySQL, AccountRepositorySQL>();
        builder.Services.AddScoped<TransactionRepository>();
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // Added logging.
        builder.Services.AddLogging();

        // Need ControllersWithViews?
        //builder.Services.AddControllersWithViews();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();

            // Delete database if I want a clean state
            //context.Database.EnsureDeleted();

            context.Database.EnsureCreated();

            // Set up account numbers.
            context.EnsureDatabaseSetup();
        }

        // Setting up custom error page including status code errors.
        app.UseExceptionHandler("/Error/Error");
        app.UseStatusCodePagesWithReExecute("/Error/Error", "?statusCode={0}");

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

        return app;
    }
}
