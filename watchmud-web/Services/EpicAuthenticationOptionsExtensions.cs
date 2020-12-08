using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Watchmud.Web.Services
{

    public static class EpicAuthenticationOptionsExtensions
    {
        public static AuthenticationBuilder AddEpic(this AuthenticationBuilder builder)
            => builder.AddEpic(EpicDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddEpic(this AuthenticationBuilder builder, Action<EpicOptions> configureOptions)
            => builder.AddEpic(EpicDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddEpic(this AuthenticationBuilder builder, string authenticationScheme,
            Action<EpicOptions> configureOptions)
            => builder.AddEpic(authenticationScheme, EpicDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddEpic(this AuthenticationBuilder builder, string authenticationScheme, string displayName,
            Action<EpicOptions> configureOptions)
            => builder.AddOAuth<EpicOptions, EpicHandler>(authenticationScheme, displayName, configureOptions);
    }
}
