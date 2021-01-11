using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Watchmud.Web.Services;

namespace Watchmud.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*
             * example pulled from
             * https://www.red-gate.com/simple-talk/dotnet/net-development/oauth-2-0-with-github-in-asp-net-core/
             */
            
            services.AddRazorPages();
            
            // authenticate via GitHub
            services.AddAuthentication(options =>
                {
                    // store oauth details via cookies
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                }).AddCookie()
                .AddGitHub(options =>
                {
                    /*
                     * These are secret!
                     *
                     * > cd watchmud-web # project directory
                     * > dotnet user-secrets init
                     * > dotnet user-secrets set "GitHub:ClientId" "secretclientid"
                     * > dotnet user-secrets set "GitHub:ClientSecret" "secretsgoeshere"
                     */
                    options.CallbackPath = new PathString("/github-oauth");
                    options.ClientId = Configuration["GitHub:ClientId"];
                    options.ClientSecret = Configuration["GitHub:ClientSecret"];
                    options.SaveTokens = true; // so we can use it later to call for stuff 
                    options.Scope.Add("user:email"); // so we can get the user's email 
                })
                .AddGoogle(options =>
                {
                    options.CallbackPath = new PathString("/google-oauth");
                    options.ClientId = Configuration["Google:ClientId"];
                    options.ClientSecret = Configuration["Google:ClientSecret"];
                    options.SaveTokens = true; // so we can use it later to call for stuff
                })
                .AddFacebook(options =>
                {
                    options.CallbackPath = new PathString("/signin-facebook");
                    options.AppId = Configuration["Facebook:ClientId"];
                    options.AppSecret = Configuration["Facebook:ClientSecret"];
                    options.SaveTokens = true; // so we can use it later to call for stuff
                    options.Scope.Add("email");
                    options.AccessDeniedPath = "/AccessDeniedPathInfo";
                })
                .AddEpic(options =>
                {
                    options.CallbackPath = new PathString("/signin-epic");
                    options.ClientId = Configuration["Epic:ClientId"];
                    options.ClientSecret = Configuration["Epic:ClientSecret"];
                    options.SaveTokens = true; 
                })
                .AddTwitch(options =>
                {
                    options.CallbackPath = new PathString("/signin-twitch");
                    options.ClientId = Configuration["Twitch:ClientId"];
                    options.ClientSecret = Configuration["Twitch:ClientSecret"];
                    options.SaveTokens = true;
                });
            
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            
            // ngrok doesn't supply forwarded headers :(
            // app.UseForwardedHeaders();
            app.Use((context, next) =>
            {
                // so here's a hack to set our request host to be our ngrok host, which changes
                // each time ngrok is restarted (unless I pay them)
                // context.Request.Scheme = "https";
                // context.Request.Host = HostString.FromUriComponent("50cfcbe16af8.ngrok.io");
                return next();
            });
            
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
