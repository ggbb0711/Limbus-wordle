using System.IO.Abstractions;
using System.Text.Json;
using Limbus_wordle.Models;
using Limbus_wordle.Services;

namespace Limbus_wordle.Services
{
    public class DailyIdentityGameModeService
    {
        private static DailyIdentityFile _dailyIdentityFile {get; set;}
        public static DailyIdentityFile GetDailyIdentityFile()
        {
            return _dailyIdentityFile;
        } 

        public static async Task Reset()
        {
            var rootLink = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootLink, Environment.GetEnvironmentVariable("DailyIdentityJSONFile"));
            var fileSystem = new FileSystem();
            var gameStateIdentity = new GameStateIdentityService(fileSystem);
            try
            {
                string dailyIdentityFile = await fileSystem.File.ReadAllTextAsync(filePath);
                if(dailyIdentityFile=="") dailyIdentityFile = "{}";
                var deserializeDailyIdentities = JsonSerializer.Deserialize<DailyIdentityFile>(dailyIdentityFile)
                    ??new DailyIdentityFile()
                    {
                        TodayIdentity = (await gameStateIdentity.GenerateNewGameState()).CorrectGuess,
                        YesterdayIdentity = (await gameStateIdentity.GenerateNewGameState()).CorrectGuess,
                    };
                if((deserializeDailyIdentities.ResetTimer-DateTime.Today).TotalDays<=0)
                {
                    deserializeDailyIdentities.ResetTimer =DateTime.Today.AddDays(1);
                    deserializeDailyIdentities.YesterdayIdentity = deserializeDailyIdentities.TodayIdentity;
                    deserializeDailyIdentities.TodayIdentity = (await gameStateIdentity.GenerateNewGameState()).CorrectGuess;
                }
                Console.WriteLine(deserializeDailyIdentities);
                await File.WriteAllTextAsync(Path.Combine(rootLink,Environment.GetEnvironmentVariable("DailyIdentityJSONFile")),JsonSerializer.Serialize(deserializeDailyIdentities));
                _dailyIdentityFile = deserializeDailyIdentities;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}