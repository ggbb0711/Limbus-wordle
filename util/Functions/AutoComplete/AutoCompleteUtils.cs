using System.IO.Abstractions;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Limbus_wordle.util.Funcitons.AutoComplete
{
    public class AutoCompleteUtils
    {
        public static async Task<List<Identity>> AutoCompleteIdentity(string term,IFile file)
        {
            var rootLink = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootLink, Environment.GetEnvironmentVariable("IdentityJSONFile"));
            string identitiesFile = "{}";
            try
            {
                identitiesFile = await file.ReadAllTextAsync(filePath);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }
            var deserializeIdentities = JsonSerializer.Deserialize<Dictionary<string,Identity>>(identitiesFile)
                ??new Dictionary<string,Identity>();
            
            List<Identity> res =[];
            foreach(var identity in deserializeIdentities)
            {
                if(identity.Value.Name.ToLower().Contains(term.ToLower()))
                {
                    res.Add(identity.Value);
                }
            }
        
            return res;
        }
    }
}