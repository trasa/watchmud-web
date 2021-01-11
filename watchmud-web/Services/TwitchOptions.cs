using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace Watchmud.Web.Services
{
    public class TwitchOptions : OAuthOptions
    {
        public TwitchOptions()
        {
            CallbackPath = new PathString("/signin-twitch");
            AuthorizationEndpoint = TwitchDefaults.AuthorizationEndpoint;
            TokenEndpoint = TwitchDefaults.TokenEndpoint;
            UserInformationEndpoint = TwitchDefaults.UserInformationEndpoint;
            Scope.Add("user:read:email");
                
            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
            ClaimActions.MapJsonKey("display_name", "display_name");
            ClaimActions.MapJsonKey(ClaimTypes.Role, "type");
            ClaimActions.MapJsonKey("broadcaster_type", "broadcaster_type");
            ClaimActions.MapJsonKey("profile_image_url", "profile_image_url");
            ClaimActions.MapJsonKey("offline_image_url", "offline_image_url");
            ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        }
    }
}