

using System.IO.Abstractions;
using System.Text.Json;
using Limbus_wordle.Services;
using Limbus_wordle.util.Functions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

public class APIController(IDataProtectionProvider dataProtectorProvider):Controller{
    private readonly IDataProtector _dataProtector = dataProtectorProvider.CreateProtector(Environment.GetEnvironmentVariable("CookiePass"));

    public async Task<IActionResult> TodayIdentity(){

        return Ok(DailyIdentityGameModeService.GetDailyIdentityFile());
    }

    public async Task<IActionResult> Random(){
        return Ok(RandomIdentity.Get());
    }

    public async Task<IActionResult> All(){
        var rootLink = Directory.GetCurrentDirectory();
        var identitiesFilePath = Path.Combine(rootLink, Environment.GetEnvironmentVariable("IdentityJSONFile"));

        string identitiesFile = await new FileSystem().File.ReadAllTextAsync(identitiesFilePath);

        var deserializeIdentities = JsonSerializer.Deserialize<Dictionary<string,Identity>>(identitiesFile)
            ??new Dictionary<string,Identity>();
        
        return Ok(deserializeIdentities);
    }
}