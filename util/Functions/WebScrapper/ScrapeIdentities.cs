using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HtmlAgilityPack;
using Limbus_wordle.Interfaces;
using Microsoft.AspNetCore.Identity;

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
            var identities = new Dictionary<string,Identity>();
            try
            {
                identitiesFile=(await File.ReadAllTextAsync(filePath))??"{}";
                identities =  JsonSerializer.Deserialize<Dictionary<string,Identity>>(identitiesFile);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            } 


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
                    var NameNodeInside = HttpUtility.HtmlDecode(characterHeader.QuerySelector(".character-details>h1").GetDirectInnerText().Replace("NEW!"," ").Trim());
                    var Name = NameNodeInside.Replace("[","").Replace("]","").Trim();
                    var IdentityIconNode = characterHeader.QuerySelector("img[loading='lazy']");
                    var IdentityIconUrl =(IdentityIconNode!=null)?"https://www.prydwen.gg" + IdentityIconNode.Attributes["data-src"].Value:"Missing";
                    var Sinner = NameNodeInside.Split("] ")[1];
                    var skills = document.DocumentNode.QuerySelectorAll(".skills-v2 .col")
                        .Take(3);
                    // var SplashArtNode = document.DocumentNode.QuerySelector("#section-gallery .gatsby-image-wrapper.gatsby-image-wrapper-constrained.full-image img[loading='lazy']");
                    // var SplashArt =(SplashArtNode!=null)?"https://www.prydwen.gg" + SplashArtNode.Attributes["data-src"].Value:"Missing";
                    List<Skill> IdentitySkills = skills.Select(skill=>new Skill()
                        {
                            SinAffinity = skill.QuerySelector(".skill-header .skill-info .skill-type.pill.limbus-affinity-box").InnerText,
                            AttackType = skill.QuerySelector(".additional-information p:nth-child(1) span").InnerText,
                            SkillCoinCount= Int32.Parse(skill.QuerySelector(".additional-information p:nth-child(3) span").InnerText),
                        }).ToList();
                    var identityIconFileName = "Missing";
                    // var splashArtFileName = SanitizeFileName(Name+"_splash.jpg");
                    if(IdentityIconNode!=null) identityIconFileName = await UploadToCloudinary(IdentityIconUrl,Name);
                    // if(SplashArtNode!=null)await DownloadImgAsync.Download(SplashArt,Path.Combine(rootLink,Environment.GetEnvironmentVariable("IdentityImgFilePath")+splashArtFileName));
                    identities[link] = new Identity()
                    {
                        Name = Name,
                        Sinner = Sinner,
                        Icon =identityIconFileName,
                        Skills = IdentitySkills
                    };
                }
            }
            await File.WriteAllTextAsync(Path.Combine(rootLink,Environment.GetEnvironmentVariable("IdentityJSONFile")),JsonSerializer.Serialize(identities));
        }

        private async Task<string> UploadToCloudinary(string url,string fileName){
            try
            {
                Cloudinary cloudinary = new(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
                cloudinary.Api.Secure = true;
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(url),
                    PublicId=fileName,
                    UseFilename = true,
                    UniqueFilename=false,
                    Overwrite = true
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
    }

}