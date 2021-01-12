using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Watchmud.Web.Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TwitchHandler : OAuthHandler<TwitchOptions>
    {
        public TwitchHandler(IOptionsMonitor<TwitchOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }


        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {

            var parameters = new Dictionary<string, string>()
            {
                {"client_id", Options.ClientId},
                {"client_secret", Options.ClientSecret},
                {"redirect_uri", context.RedirectUri},
                {"grant_type", "authorization_code"},
                {"code", context.Code},
                {"scope", string.Join(" ", Options.Scope)}
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, QueryHelpers.AddQueryString(Options.TokenEndpoint, parameters));
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await Backchannel.SendAsync(requestMessage, Context.RequestAborted);
            if (response.IsSuccessStatusCode)
            {
                string body = new StreamReader(await response.Content.ReadAsStreamAsync()).ReadToEnd(); // TODO merge into next line
                var payload = JsonDocument.Parse(body);
                return OAuthTokenResponse.Success(payload);
            }
            else
            {
                var error = "OAuth token endpoint failure: " + await Display(response);
                return OAuthTokenResponse.Failed(new Exception(error));
            }
        }
        
        
        private static async Task<string> Display(HttpResponseMessage response)
        {
            var output = new StringBuilder();
            output.Append("Status: " + response.StatusCode + ";");
            output.Append("Headers: " + response.Headers + ";");
            output.Append("Body: " + await response.Content.ReadAsStringAsync() + ";");
            return output.ToString();
        }
        
        protected override async Task<AuthenticationTicket> CreateTicketAsync(
            ClaimsIdentity identity,
            AuthenticationProperties properties,
            OAuthTokenResponse tokens)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
            request.Headers.Add("Client-Id", Options.ClientId);
            var response = await Backchannel.SendAsync(request, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"An error occurred when retrieving user information ({response.StatusCode}). Please check if the authentication information is correct.");
            }

            using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            // payload:
            //   { "data": [ {
            //                "id": "1234", (etc)
            //               }
            //             ] 
            //    }
            JsonElement userData = payload.RootElement.GetProperty("data")[0];
            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, userData);
            context.RunClaimActions();
            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }
    }
}