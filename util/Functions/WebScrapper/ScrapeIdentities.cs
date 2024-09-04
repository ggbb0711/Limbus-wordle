using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Limbus_wordle.Interfaces;
using Limbus_wordle.util.Functions.DownloadImg;

namespace Limbus_wordle.util.WebScrapper
{
    public class ScrapeIdentities : IScrapper
    {
        public async Task ScrapAsync()
        {
            var web = new HtmlWeb();
            var document = web.Load("https://www.prydwen.gg/limbus-company/identities");

            var rootLink = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootLink, Environment.GetEnvironmentVariable("IdentityJSONFile"));
            
            string identitiesFile = "{}";
            try
            {
                identitiesFile=await File.ReadAllTextAsync(filePath);
            }
            catch (System.Exception)
            {
            } 

            var identities =  JsonSerializer.Deserialize<Dictionary<string,Identity>>(identitiesFile)?? 
                new Dictionary<string, Identity>();

            //Get the links to all identities
            var identitiesLinks= document.DocumentNode.QuerySelectorAll(".employees-container .avatar-card.card a")
                .Select(n=>"https://www.prydwen.gg"+n.Attributes["href"].Value)
                .ToList();
            foreach(var link in identitiesLinks)
            {
                if(!identities.ContainsKey(link))
                {
                    Console.WriteLine("Entering: "+link);
                    document = web.Load(link);
                    var characterHeader = document.DocumentNode.QuerySelector(".character-header.ag");
                    var Name = characterHeader.QuerySelector(".character-details>h1").InnerText.Replace("NEW!"," ").Replace("[","").Replace("]","").Trim();
                    var IdentityIconNode = characterHeader.QuerySelector("img[loading='lazy']");
                    var IdentityIcon =(IdentityIconNode!=null)?"https://www.prydwen.gg" + IdentityIconNode.Attributes["data-src"].Value:"Missing";
                    var Sinner = document.DocumentNode.QuerySelector("#section-profile .info-list.row .info-list-row:nth-child(3) .details").InnerText;
                    var skills = document.DocumentNode.QuerySelectorAll(".skills.row .skill-box.hsr")
                        .Take(3);
                    var SplashArtNode = document.DocumentNode.QuerySelector("#section-gallery .gatsby-image-wrapper.gatsby-image-wrapper-constrained.full-image img[loading='lazy']");
                    var SplashArt =(SplashArtNode!=null)?"https://www.prydwen.gg" + SplashArtNode.Attributes["data-src"].Value:"Missing";
                    List<Skill> IdentitySkills = skills.Select(skill=>new Skill()
                        {
                            SinAffinity = skill.QuerySelector(".skill-header .skill-type.pill.limbus-affinity-box").InnerText,
                            AttackType = skill.QuerySelector(".skill-details .info-row:nth-child(1) .details").InnerText,
                            SkillCoinCount= Int32.Parse(skill.QuerySelector(".skill-details .info-row:nth-child(3) .details").InnerText),
                        }).ToList();
                    var identityIconFileName = SanitizeFileName(Name+"_icon.jpg");
                    var splashArtFileName = SanitizeFileName(Name+"_splash.jpg");
                    if(IdentityIconNode!=null)await DownloadImgAsync.Download(IdentityIcon,Path.Combine(rootLink,Environment.GetEnvironmentVariable("IdentityImgFilePath")+identityIconFileName));
                    if(SplashArtNode!=null)await DownloadImgAsync.Download(SplashArt,Path.Combine(rootLink,Environment.GetEnvironmentVariable("IdentityImgFilePath")+splashArtFileName));
                    identities[link] = new Identity()
                    {
                        Name = Name,
                        Sinner = Sinner,
                        Icon =(IdentityIconNode!=null)? '/'+Environment.GetEnvironmentVariable("IdentityImgFilePath")+identityIconFileName:"Missing",
                        SplashArt =(SplashArtNode!=null)? '/'+Environment.GetEnvironmentVariable("IdentityImgFilePath")+splashArtFileName:"Missing",
                        Skills = IdentitySkills
                    };
                }
            }
            await File.WriteAllTextAsync(Path.Combine(rootLink,Environment.GetEnvironmentVariable("IdentityJSONFile")),JsonSerializer.Serialize(identities));
        }
        private string SanitizeFileName(string fileName)
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars());
            string invalidReStr = string.Format("[{0}]", Regex.Escape(invalidChars));
            return Regex.Replace(fileName, invalidReStr, "_");
        }
    }

}