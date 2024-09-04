using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Limbus_wordle.Models;
using System.IO.Abstractions;
using Limbus_wordle.Services;
using Limbus_wordle.util.Funcitons.AutoComplete;
using Microsoft.AspNetCore.DataProtection;

namespace Limbus_wordle.Controllers;

public class EndlessIdentityModeController : Controller
{
    private readonly IFileSystem _fileSystem;
    private readonly GameStateIdentityService _gameStateIdentityService;
    private readonly IDataProtector _dataProtector;
    public EndlessIdentityModeController( IFileSystem fileSystem, GameStateIdentityService gameStateIdentityService, IDataProtectionProvider dataProtectorProvider)
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
        var gameMode = HttpContext.Items["DecryptedGameMode"] as EndlessGameMode<Identity>;
        gameMode??=new EndlessGameMode<Identity>()
        {
            GameState = await _gameStateIdentityService.GenerateNewGameState(),
        };

        bool winCoditionBefore = gameMode.GameState.HasWon;
        var gameState = await _gameStateIdentityService.Guess(gameMode.GameState,guess);
        
        if(!winCoditionBefore&&gameState.HasWon)
        {
            gameMode.WinStreak+=1;
            gameMode.BestScore=Math.Max(gameMode.WinStreak,gameMode.BestScore);
        }
        if(!winCoditionBefore&&!gameState.HasWon&&gameState.GameOver) gameMode.WinStreak=0;
        
        gameMode.GameState=gameState;
        return Ok(_dataProtector.Protect(System.Text.Json.JsonSerializer.Serialize(gameMode)));
    }

    [HttpPost]
    public async Task<IActionResult> NewGame( )
    {
        var gameMode = HttpContext.Items["DecryptedGameMode"] as EndlessGameMode<Identity>?? new EndlessGameMode<Identity>();
        gameMode.GameState = await _gameStateIdentityService.GenerateNewGameState();
        return Ok(_dataProtector.Protect(System.Text.Json.JsonSerializer.Serialize(gameMode)));
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GuessTableIdentity( )
    {
        var gameMode = HttpContext.Items["DecryptedGameMode"] as EndlessGameMode<Identity>;
        gameMode??=new EndlessGameMode<Identity>()
        {
            GameState = await _gameStateIdentityService.GenerateNewGameState(),
        };

        return PartialView("_GuessTableIdentity",gameMode.GameState);
    }

    [HttpPost]
    public async Task<IActionResult> InputContinueButton( )
    {
        var gameMode = HttpContext.Items["DecryptedGameMode"] as EndlessGameMode<Identity>;
        gameMode??=new EndlessGameMode<Identity>()
        {
            GameState = await _gameStateIdentityService.GenerateNewGameState(),
        };
        ViewBag.GameOver = gameMode.GameState.GameOver;
        return PartialView("_InputContinueButton");
    }

    [HttpPost]
    public async Task<IActionResult> Result( )
    {
        var gameMode = HttpContext.Items["DecryptedGameMode"] as EndlessGameMode<Identity>;
        gameMode??=new EndlessGameMode<Identity>()
        {
            GameState = await _gameStateIdentityService.GenerateNewGameState(),
        };
        return PartialView("_IdentityResult",gameMode.GameState);
    }

    [HttpPost]
    public async Task<IActionResult> Footer( )
    {
        var gameMode = HttpContext.Items["DecryptedGameMode"] as EndlessGameMode<Identity>;
        gameMode??=new EndlessGameMode<Identity>()
        {
            GameState = await _gameStateIdentityService.GenerateNewGameState(),
        };
        ViewBag.BestScore = gameMode.BestScore;
        ViewBag.WinStreak = gameMode.WinStreak;
        return PartialView("_EndlessModeIdentityFooter");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
