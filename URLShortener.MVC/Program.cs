using Microsoft.EntityFrameworkCore;
using URLShortener.MVC.Data;
using URLShortener.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register UrlShortenerService
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();

var app = builder.Build();

// Middleware kiểm tra shortlink với prefix /s/
app.Use(async (context, next) =>
{   
    var path = context.Request.Path.Value?.Trim('/');
    if (!string.IsNullOrEmpty(path) && path.StartsWith("s/"))
    {
        // Lấy shortUrl sau prefix "s/"
        var shortUrl = path.Substring(2); // Bỏ "s/"
        
        var db = context.RequestServices.GetRequiredService<DataContext>();
        // Case-insensitive lookup in middleware
        var entry = db.HomeModels.FirstOrDefault(x => 
            x.ShortenedUrl.ToLower() == shortUrl.ToLower());
        if (entry != null)
        {
            context.Response.Redirect(entry.OriginalUrl);
            return;
        }
    }
    await next();
});

// Routing
app.UseStaticFiles();
app.UseRouting();
app.MapDefaultControllerRoute();
app.Run();
