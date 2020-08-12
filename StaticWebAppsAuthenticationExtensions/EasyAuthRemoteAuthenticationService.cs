using AzureStaticWebApps.Blazor.Authentication;
using AzureStaticWebApps.Blazor.Authentication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StaticWebAppsAuthenticationExtensions
{
    public class EasyAuthRemoteAuthenticationService : AuthenticationStateProvider, IRemoteAuthenticationService<RemoteAuthenticationState>
    {
        public RemoteAuthenticationOptions<EasyAuthOptions> Options { get; }
        public HttpClient HttpClient { get; }

        public EasyAuthRemoteAuthenticationService(IOptions<RemoteAuthenticationOptions<EasyAuthOptions>> options, NavigationManager navigationManager)
        {
            Options = options.Value;
            HttpClient = new HttpClient() { BaseAddress = new Uri(navigationManager.BaseUri) };
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // TODO: Cache this so that it doesn't probe the host every time.
            try
            {
                var authDataUrl = Options.ProviderOptions.AuthenticationDataUrl + "/.auth/me";
                var data = await HttpClient.GetFromJsonAsync<AuthenticationData>(authDataUrl);

                var principal = data.ClientPrincipal;
                principal.UserRoles = principal.UserRoles.Except(new string[] { "anonymous" }, StringComparer.CurrentCultureIgnoreCase);

                if (!principal.UserRoles.Any())
                {
                    return new AuthenticationState(new ClaimsPrincipal());
                }

                var identity = new ClaimsIdentity(principal.IdentityProvider);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principal.UserId));
                identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserDetails));
                identity.AddClaims(principal.UserRoles.Select(r => new Claim(ClaimTypes.Role, r)));
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }
        }

        public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            throw new NotImplementedException();
        }

        public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignOutAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            throw new NotImplementedException();
        }

        public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            throw new NotImplementedException();
        }

        public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignOutAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            throw new NotImplementedException();
        }
    }
}
