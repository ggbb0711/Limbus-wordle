

using System.IO.Abstractions;
using System.Text.Json;

namespace Limbus_wordle.util.Functions{
    public class RandomIdentity{
        public static async Task<Identity> Get(){
            var rootLink = Directory.GetCurrentDirectory();
            var identitiesFilePath = Path.Combine(rootLink, Environment.GetEnvironmentVariable("IdentityJSONFile"));

            try
            {
                string identitiesFile = await new FileSystem().File.ReadAllTextAsync(identitiesFilePath);
                Random random = new Random();

                var deserializeIdentities = JsonSerializer.Deserialize<Dictionary<string,Identity>>(identitiesFile)
                    ??new Dictionary<string,Identity>();
                
                return deserializeIdentities.ElementAt(random.Next(deserializeIdentities.Count))
                                    .Value;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}