using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Watchmud.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUri = "/")
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = returnUri
            });
        }
    }
}