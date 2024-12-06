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

        builder.Services.AddScoped<UserController>();
        builder.Services.AddScoped<PasswordService>();
        builder.Services.AddScoped<IUserRepositorySQL, UserRepositorySQL>();

        builder.Services.AddScoped<ILogger<UserRepositorySQL>, Logger<UserRepositorySQL>>();
        builder.Services.AddControllersWithViews();

        builder.Services.AddMemoryCache();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        //Configure the HTTP request pipeline.
        // if (!app.Environment.IsDevelopment())
        // {
        //     app.UseStatusCodePages();
        //     app.UseExceptionHandler("/Error");
        //     app.UseHsts();
        //     //app.UseDeveloperExceptionPage();
        //     //app.UseExceptionHandler("/Error");
        //     //app.UseExceptionHandler("/Home/Error");
        //     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        //     //app.UseHsts();
        // }
        // else
        // {
        //     app.UseStatusCodePages();
        //     app.UseDeveloperExceptionPage();
        //     app.UseExceptionHandler("/Error");
        //     app.UseHsts();
        // }
        //app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
        }

        app.UseExceptionHandler("/Error");
        // app.UseStatusCodePages();
        app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");
        //app.UseStatusCodePagesWithRedirects("/Error{0}");

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        //app.MapRazorPages();
        app.MapRazorPages();
        app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

        return app;
    }
}
