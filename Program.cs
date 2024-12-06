using FinanceApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.ConfigureServices().ConfigurePipeline();

app.Run();
