namespace Watchmud.Web.Services
{
    public class EpicDefaults
    {
        public const string AuthenticationScheme = "Epic";

        public static readonly string DisplayName = "Epic";

        // https://dev.epicgames.com/docs/services/en-US/EpicAccountServices/GettingStarted/index.html
        // https://www.epicgames.com/id/authorize?client_id={client_id}&response_type=code&scope=basic_profile&redirect_uri=https://www.example.com&state=rfGWJux2WL86Zxr6nKApCAnDo8KexEUE

        public static readonly string AuthorizationEndpoint = "https://www.epicgames.com/id/authorize";
        
        public static readonly string TokenEndpoint = "https://api.epicgames.dev/epic/oauth/v1/token";

        public static readonly string UserInformationEndpoint = "https://api.epicgames.dev/epic/oauth/v1/userInfo";
    }
}