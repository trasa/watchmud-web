using System;
using System.Globalization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace Watchmud.Web.Services
{
    public class EpicOptions : OAuthOptions
    {
        public EpicOptions()
        {
            CallbackPath = new PathString("/signin-facebook");
            AuthorizationEndpoint = EpicDefaults.AuthorizationEndpoint;
            TokenEndpoint = EpicDefaults.TokenEndpoint;
            UserInformationEndpoint = EpicDefaults.UserInformationEndpoint; 
            Scope.Add("basic_profile");
            Scope.Add("presence");
            Scope.Add("friends_list");

            ClaimActions.MapJsonKey("accountId", "sub");
            ClaimActions.MapJsonKey("userName", "preferred_username");
        }
    }
}