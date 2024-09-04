using System.IO.Abstractions;
using dotenv.net;
using Limbus_wordle.BackgroundTask;
using Limbus_wordle.Middleware;
using Limbus_wordle.Models;
using Limbus_wordle.Services;
using Limbus_wordle.util.WebScrapper;
using Microsoft.Extensions.FileProviders;


DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ScrapeIdentities>(); 
builder.Services.AddHostedService<BackgroundScrapeData>();
builder.Services.AddHostedService<BackgroundResetDailyIdentityMode>(); 
builder.Services.AddTransient<IFileSystem,FileSystem>();
builder.Services.AddTransient<GameStateIdentityService>();
builder.Services.AddDataProtection();

var app = builder.Build();

app.UseStaticFiles(); // For the wwwroot folder

app.UseStaticFiles(new StaticFileOptions(){
FileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), @"Content")),
    RequestPath = new PathString("/Content")
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseWhen(ctx=>ctx.Request.Path.StartsWithSegments("/EndlessIdentityMode"),app=>
{
    app.UseDecryptGameMode<EndlessGameMode<Identity>>();
});

app.UseWhen(ctx=>ctx.Request.Path.StartsWithSegments("/DailyIdentityMode"),app=>
{
    app.UseDecryptGameMode<DailyGameMode<Identity>>();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DailyIdentityMode}/{action=Index}/{id?}");


app.Run();
