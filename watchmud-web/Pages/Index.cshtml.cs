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
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        
        public IReadOnlyList<Repository> Repositories { get; set; }
        public IReadOnlyList<Repository> StarredRepositories { get; set; }
        public IReadOnlyList<User> Followers { get; set; }
        public IReadOnlyList<User> Following { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                string accessToken = await HttpContext.GetTokenAsync("access_token");
                var github = new GitHubClient(new ProductHeaderValue("AspNetCoreGitHubAuth"), new InMemoryCredentialStore(new Credentials(accessToken)));
                Repositories = await github.Repository.GetAllForCurrent();
                StarredRepositories = await github.Activity.Starring.GetAllForCurrent();
                Followers = await github.User.Followers.GetAllForCurrent();
                Following = await github.User.Followers.GetAllFollowingForCurrent();
                var user = await github.User.Current();
                // if the user has a public email set, then this shows here:
                _logger.LogInformation($"user public email is {user.Email}");
                
                /* Throws exception because we dont have access to /users/email
                   (technically returns 404 and a NotFoundException, 
                   https://github.com/octokit/octokit.net/issues/1010
                   )
                 */
                /*
                var emails = await github.User.Email.GetAll();
                _logger.LogInformation($"email count {emails.Count}");
                foreach(EmailAddress email in emails)
                {
                    _logger.LogInformation($"{email.Email} {email.Primary} {email.Verified} {email.Visibility}");
                }
                */
            }
        }
    }
}
