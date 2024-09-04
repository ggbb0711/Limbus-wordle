using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Limbus_wordle.Models;
using System.IO.Abstractions;
using Limbus_wordle.Services;
using Limbus_wordle.util.Funcitons.AutoComplete;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace Limbus_wordle.Controllers;

public class DailyIdentityModeController : Controller
{
    private readonly IFileSystem _fileSystem;
    private readonly GameStateIdentityService _gameStateIdentityService;
    private readonly IDataProtector _dataProtector;
    public DailyIdentityModeController( IFileSystem fileSystem, GameStateIdentityService gameStateIdentityService, IDataProtectionProvider dataProtectorProvider)
    {
        _fileSystem = fileSystem;
        _gameStateIdentityService = gameStateIdentityService;
        _dataProtector = dataProtectorProvider.CreateProtector(Environment.GetEnvironmentVariable("CookiePass"));
    }

    public async Task<JsonResult> AutoComplete(string term="")
    {
       return Json(await AutoCompleteUtils.AutoCompleteIdentity(term,_fileSystem.File));
    }

    [HttpPost]
    public async Task<IActionResult> Guess(string guess="")
    {
        var gameMode = GetDailyGameMode();

        bool winCoditionBefore = gameMode.GameState.HasWon;
        var gameState = await _gameStateIdentityService.Guess(gameMode.GameState,guess);
        
        if(!winCoditionBefore&&gameState.HasWon)
        {
            gameMode.WinStreak+=1;
            gameMode.BestScore=Math.Max(gameMode.WinStreak,gameMode.BestScore);
        }
        if(!winCoditionBefore&&!gameState.HasWon&&gameState.GameOver) gameMode.WinStreak=0;
        
        gameMode.GameState=gameState;
        gameMode.GuessDate=DateTime.Now;
        return Ok(_dataProtector.Protect(System.Text.Json.JsonSerializer.Serialize(gameMode)));
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GuessTableIdentity( )
    {
        var gameMode = GetDailyGameMode();
        return PartialView("_GuessTableIdentity",gameMode.GameState);
    }

    [HttpPost]
    public async Task<IActionResult> InputContinueButton( )
    {
        var gameMode = GetDailyGameMode();
        if(gameMode.GameState.GameOver) return Ok("<p>Return tomorrow for another game</p>");
        ViewBag.GameOver=gameMode.GameState.GameOver;
        return PartialView("_InputContinueButton");
    }

    [HttpPost]
    public async Task<IActionResult> Result()
    {
        var gameMode = GetDailyGameMode();
        return PartialView("_IdentityResult",gameMode.GameState);
    }

    [HttpPost]
    public async Task<IActionResult> Footer( )
    {
        var gameMode = GetDailyGameMode();
        ViewBag.BestScore = gameMode.BestScore;
        ViewBag.WinStreak = gameMode.WinStreak;
        ViewBag.YesterdayIdentity = DailyIdentityGameModeService.GetDailyIdentityFile().YesterdayIdentity;
        return PartialView("_DailyModeIdentityFooter");
    }

    private DailyGameMode<Identity> GetDailyGameMode()
    {
        var dailyIdentityFile = DailyIdentityGameModeService.GetDailyIdentityFile();
        var gameMode = HttpContext.Items["DecryptedGameMode"] as DailyGameMode<Identity>;
        gameMode??=new DailyGameMode<Identity>()
        {
            GameState = new GameState<Identity>()
            {
                CorrectGuess = dailyIdentityFile.TodayIdentity,
            },
        };
        if(Math.Abs((dailyIdentityFile.ResetTimer-gameMode.GuessDate).TotalDays)>1)
        {
            gameMode.GameState =new GameState<Identity>()
            {
                CorrectGuess = dailyIdentityFile.TodayIdentity,
            };
        } 

        return gameMode;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
