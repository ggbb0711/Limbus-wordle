

using System.Text.Json;
using Limbus_wordle.Services;
using Limbus_wordle.util.Functions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

public class APIController(IDataProtectionProvider dataProtectorProvider):Controller{
    private readonly IDataProtector _dataProtector = dataProtectorProvider.CreateProtector(Environment.GetEnvironmentVariable("CookiePass"));

    public async Task<IActionResult> TodayIdentity(){

        return Ok(_dataProtector.Protect(JsonSerializer.Serialize(DailyIdentityGameModeService.GetDailyIdentityFile())));
    }

    public async Task<IActionResult> Random(){
        return Ok(_dataProtector.Protect(JsonSerializer.Serialize(RandomIdentity.Get())));
    }
}