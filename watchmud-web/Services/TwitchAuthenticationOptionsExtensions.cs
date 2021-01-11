using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Watchmud.Web.Services
{

    public static class TwitchAuthenticationOptionsExtensions
    {
        public static AuthenticationBuilder AddTwitch(this AuthenticationBuilder builder)
            => builder.AddTwitch(TwitchDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddTwitch(this AuthenticationBuilder builder, Action<TwitchOptions> configureOptions)
            => builder.AddTwitch(TwitchDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddTwitch(this AuthenticationBuilder builder, string authenticationScheme,
            Action<TwitchOptions> configureOptions)
            => builder.AddTwitch(authenticationScheme, TwitchDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddTwitch(this AuthenticationBuilder builder, string authenticationScheme, string displayName,
            Action<TwitchOptions> configureOptions)
            => builder.AddOAuth<TwitchOptions, TwitchHandler>(authenticationScheme, displayName, configureOptions);
    }
}
