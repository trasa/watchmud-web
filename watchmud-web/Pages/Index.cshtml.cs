using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Internal;

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
        public string GitHubUserPublicEmail { get; set; }
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
                AuthenticationSource = User.Identity.AuthenticationType;
                if (User.Identity.AuthenticationType == "GitHub")
                {
                    await GitHubAsync(accessToken);
                } else if (User.Identity.AuthenticationType == "Google")
                {
                    GoogleAsync(accessToken);
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
            GitHubUserPublicEmail = user.Email;

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

        private void GoogleAsync(string accessToken)
        {
            logger.LogInformation("do google async here");
            GitHubUserPublicEmail = "google auth stuff here";
        }
    }
}
