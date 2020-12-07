using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Facebook;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Octokit;
using Octokit.Internal;
using Watchmud.Web.Models;

namespace WebApplication1.Pages
{
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
        public String ClaimedEmail { get; set; }
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
                ClaimedEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                switch (User.Identity.AuthenticationType)
                {
                    case "GitHub":
                        await GitHubAsync(accessToken);
                        break;
                    case "Google":
                        await GoogleAsync(accessToken);
                        break;
                    case "Facebook":
                        await FacebookAsync(accessToken);
                        break;
                }
            }
        }

        private async Task GitHubAsync(string accessToken)
        {
            // can the user info be safely retrieved via User.Claims?
            // -- looks like only the Public Email is included as a claim, and that value
            //    is optional for the user to set (they don't have to specify a public email)
            
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
            var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken);

            UserPublicEmail = payload.Email;
            Emails = new List<EmailModel>
            {
                new EmailModel
                {
                    Email = payload.Email,
                    Verified = payload.EmailVerified,
                    Primary = true,
                    Visibility = "N/A",
                }
            };
        }

        private async Task FacebookAsync(string accessToken)
        {
            // Facebook is a bit different because it allows the user
            // to override which permissions are given to you, and still 
            // complete the login to the FB App. So we ASK for the email,
            // but that doesn't mean we'll actually get it...
            var client = new FacebookClient(accessToken);
            
            // two choices here: if we don't get "email" back, assume that the permission has been declined
            // or, call /me/permissions and examine the set of permissions that was returned...
            dynamic userInfo = await client.GetTaskAsync("/me?fields=id,name,email");
            
            // {"data":[{"permission":"public_profile","status":"granted"},{"permission":"email","status":"declined"}]}
            dynamic perms = await client.GetTaskAsync("/me/permissions");
            string permsStr = perms.ToString();
            var permObj = JsonConvert.DeserializeObject<FacebookData<FacebookPermission>>(permsStr);
            bool hasEmailPermission = permObj.Data.Any(p => p.Granted && p.Permission == "email");

            if (hasEmailPermission)
            {
                UserPublicEmail = userInfo.email;
                Emails = new List<EmailModel>
                {
                    new EmailModel
                    {
                        Email = userInfo.email,
                        Visibility = "N/A",
                    }
                };
            }
            else
            {
                // redirect to login-fail, or something...
                UserPublicEmail = "Email permission denied!";
                Emails = new List<EmailModel>();
            }
        }
    }
}
