using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Watchmud.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult LoginGoogle(string returnUri = "/")
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = returnUri,
            }, "Google");
        }
        
        [HttpGet]
        public IActionResult LoginGitHub(string returnUri = "/")
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = returnUri,
            }, "GitHub");
        }
    }
}