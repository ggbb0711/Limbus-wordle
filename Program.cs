using System.IO.Abstractions;
using dotenv.net;
using Limbus_wordle.BackgroundTask;
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
builder.Services.AddDataProtection();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowOrigin",
                      policy  =>
                      {
                          policy.WithOrigins(Environment.GetEnvironmentVariable("FRONTEND_URL"));
                      });
});

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

app.UseCors("AllowOrigin");

app.UseAuthorization();

// app.UseWhen(ctx=>ctx.Request.Path.StartsWithSegments("/EndlessIdentityMode"),app=>
// {
//     app.UseDecryptGameMode<EndlessGameMode<Identity>>();
// });

// app.UseWhen(ctx=>ctx.Request.Path.StartsWithSegments("/DailyIdentityMode"),app=>
// {
//     app.UseDecryptGameMode<DailyGameMode<Identity>>();
// });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");


app.Run();
