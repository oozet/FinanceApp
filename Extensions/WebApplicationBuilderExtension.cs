using FinanceApp.Controllers;
using FinanceApp.Data;
using FinanceApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();
        builder.Services.AddAuthorization();

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddRazorPages();

        // Adding a cache.
        builder.Services.AddMemoryCache();

        // Adding application services.
        builder.Services.AddScoped<UserController>();
        builder.Services.AddScoped<PasswordService>();
        builder.Services.AddScoped<IUserRepositorySQL, UserRepositorySQL>();

        // TODO: Simplify logger? Global possible?
        builder.Services.AddScoped<ILogger<UserRepositorySQL>, Logger<UserRepositorySQL>>();
        builder.Services.AddControllersWithViews();

        builder.Services.AddMemoryCache();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();

            // Set up account numbers.
            context.EnsureDatabaseSetup();

            // Delete database if I want a clean state
            context.Database.EnsureDeleted();

            context.Database.EnsureCreated();
        }

        // Setting up custom error page including status code errors.
        app.UseExceptionHandler("/Error");
        app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");

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
