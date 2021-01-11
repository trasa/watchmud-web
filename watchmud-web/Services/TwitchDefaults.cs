namespace Watchmud.Web.Services
{
    public static class TwitchDefaults
    {
        public const string AuthenticationScheme = "Twitch";

        public static readonly string DisplayName = "Twitch";
        
   
        public static readonly string AuthorizationEndpoint = "https://id.twitch.tv/oauth2/authorize";
        
        public static readonly string TokenEndpoint = "https://id.twitch.tv/oauth2/token";

        public static readonly string UserInformationEndpoint = "https://api.twitch.tv/helix/users";
    }
}