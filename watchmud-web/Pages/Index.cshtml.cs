using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Octokit;
using Octokit.Internal;

namespace WebApplication1.Pages
{
    /*
     * res: {
"id": "102171105725265301028",
"email": "trasa@meancat.com",
"verified_email": true,
"name": "Tony Rasa",
"given_name": "Tony",
"family_name": "Rasa",
"link": "https://plus.google.com/102171105725265301028",
"picture": "https://lh3.googleusercontent.com/a-/AOh14GhjQ9PK4zNgPtf8jPFLdPKIXKksetebiqAYVnnTzg=s96-c",
"gender": "male",
"locale": "en",
"hd": "meancat.com"
}
     */
    public class GoogleUserInfo
    {
        public string Id { get; set; }
        public string Email { get; set;}
        
        [JsonProperty("verified_email")]
        public bool VerifiedEmail { get; set; }
        
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Link { get; set; }
        public string Picture { get; set; }
        public string Gender { get; set; }
        public string Locale { get; set; }
        public string HD { get; set; }
    }
    
    
    public class EmailModel {
        public string Email { get; set; }
        public bool Verified { get; set; }
        public bool Primary { get; set; }
        public string Visibility { get; set; }

        public override string ToString()
        {
            return $@"{this.Email} - Verified: {Verified} - Primary: {Primary} - Visibility: {Visibility}";
        }
    }
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;
        public string UserPublicEmail { get; set; }
        public IEnumerable<EmailModel> Emails { get; set; }
        public string AuthenticationSource { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            this.logger = logger;
        }

        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                string accessToken = await HttpContext.GetTokenAsync("access_token");
                this.AuthenticationSource = User.Identity.AuthenticationType;
                if (User.Identity.AuthenticationType == "GitHub")
                {
                    await GitHubAsync(accessToken);
                } else if (User.Identity.AuthenticationType == "Google")
                {
                    await GoogleAsync(accessToken);
                }
            }
        }

        private async Task GitHubAsync(string accessToken)
        {
            var github = new GitHubClient(new ProductHeaderValue("AspNetCoreGitHubAuth"),
                new InMemoryCredentialStore(new Credentials(accessToken)));
            // Repositories = await github.Repository.GetAllForCurrent();
            // StarredRepositories = await github.Activity.Starring.GetAllForCurrent();
            // Followers = await github.User.Followers.GetAllForCurrent();
            // Following = await github.User.Followers.GetAllFollowingForCurrent();
            var user = await github.User.Current();
            // if the user has a public email set, then this shows here:
            logger.LogInformation($"user github public email is {user.Email}");
            UserPublicEmail = user.Email;

            var emails = await github.User.Email.GetAll();
            logger.LogInformation($"email count {emails.Count}");
            Emails = from email in emails
                select new EmailModel
                {
                    Email = email.Email,
                    Verified = email.Verified,
                    Primary = email.Primary,
                    Visibility = email.Visibility.ToString() ?? "null"
                };
        }

        private async Task GoogleAsync(string accessToken)
        {
            // TODO use dependency injection
            using (var client = new HttpClient())
            {
                // I couldn't find a way to do this call from within the Google C# Lib and
                // avoid using HttpClient and JSON deserialization. It's likely that there's 
                // already a helper method somewhere that does this...
                // Elsewise this should be wrapped into some sort of helper service and use
                // DI and all that other good stuff.
                string json = await client.GetStringAsync("https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + accessToken);
                GoogleUserInfo userInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(json);
                UserPublicEmail = userInfo.Email;
                Emails = new List<EmailModel>
                {
                    new EmailModel
                    {
                        Email = userInfo.Email,
                        Verified = userInfo.VerifiedEmail,
                        Primary = true,
                        Visibility = "N/A",
                    }
                };
            }
        }
    }
}
