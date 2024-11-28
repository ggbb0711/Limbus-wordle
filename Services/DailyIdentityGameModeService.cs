using System.IO.Abstractions;
using System.Text.Json;
using Limbus_wordle.Models;
using Limbus_wordle.Services;
using Limbus_wordle.util.Functions;

namespace Limbus_wordle.Services
{
    public class DailyIdentityGameModeService(IFileSystem fileSystem)
    {
        private static DailyIdentityFile _dailyIdentityFile {get; set;}

        public static DailyIdentityFile GetDailyIdentityFile()
        {
            return _dailyIdentityFile;
        } 

        public static async Task Reset()
        {
            var rootLink = Directory.GetCurrentDirectory();
            var dailyIdentityFilePath = Path.Combine(rootLink, Environment.GetEnvironmentVariable("DailyIdentityJSONFile"));
            var fileSystem = new FileSystem();
            try
            {
                string dailyIdentityFile = await fileSystem.File.ReadAllTextAsync(dailyIdentityFilePath);

                if(dailyIdentityFile=="") dailyIdentityFile = "{}";
                var deserializeDailyIdentities = JsonSerializer.Deserialize<DailyIdentityFile>(dailyIdentityFile)
                    ??new DailyIdentityFile()
                    {
                        TodayID = new Guid().ToString(),
                        TodayIdentity = await RandomIdentity.Get(),
                        YesterdayIdentity = await RandomIdentity.Get(),
                    };


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