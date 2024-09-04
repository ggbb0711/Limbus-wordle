namespace Limbus_wordle.Middleware;  

using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using Limbus_wordle.Models;

public class DecryptGameModeMiddleware<GameMode>
{
    private readonly RequestDelegate _next;

    public DecryptGameModeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IDataProtectionProvider dataProtectionProvider)
    {
        var dataProtector = dataProtectionProvider.CreateProtector(Environment.GetEnvironmentVariable("CookiePass"));
        if (context.Request.Method == HttpMethods.Post && context.Request.Form.ContainsKey("encryptedGameMode"))
        {
            try
            {
                var encryptedGameMode = context.Request.Form["encryptedGameMode"];
                var decryptedData = dataProtector.Unprotect(encryptedGameMode);
                var gameMode = JsonConvert.DeserializeObject<GameMode>(decryptedData);
                // Attach the deserialized gameMode to the context for later use
                context.Items["DecryptedGameMode"] = gameMode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decrypting game mode: {ex.Message}");
            }
        }

        await _next(context);
    }
}

public static class DecryptGameModeMiddlewareExtensions
{
    public static IApplicationBuilder UseDecryptGameMode<GameMode>(this IApplicationBuilder app)
    {
        return app.UseMiddleware<DecryptGameModeMiddleware<GameMode>>();
    }
}