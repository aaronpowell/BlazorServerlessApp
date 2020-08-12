using AzureStaticWebApps.Blazor.Authentication;
using AzureStaticWebApps.Blazor.Authentication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StaticWebAppsAuthenticationExtensions
{
    public class EasyAuthRemoteAuthenticationService : AuthenticationStateProvider, IRemoteAuthenticationService<RemoteAuthenticationState>
    {
        public RemoteAuthenticationOptions<EasyAuthOptions> Options { get; }
        public HttpClient HttpClient { get; }
        public NavigationManager Navigation { get; }
        public IJSRuntime JSRuntime { get; }

        public EasyAuthRemoteAuthenticationService(
            IOptions<RemoteAuthenticationOptions<EasyAuthOptions>> options,
            NavigationManager navigationManager,
            IJSRuntime jsRuntime)
        {
            Options = options.Value;
            HttpClient = new HttpClient() { BaseAddress = new Uri(navigationManager.BaseUri) };
            Navigation = navigationManager;
            JSRuntime = jsRuntime;
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

        public async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            if (!(context is EasyAuthRemoteAuthenticationContext easyAuthContext))
            {
                throw new InvalidOperationException("Not an easyauthcontext");
            }
            var stateId = Guid.NewGuid().ToString();
            await JSRuntime.InvokeVoidAsync("sessionStorage.setItem", $"Blazor.EasyAuth.{stateId}", JsonSerializer.Serialize(context.State));
            Navigation.NavigateTo($"/.auth/login/{easyAuthContext.SelectedProvider}?post_login_redirect_uri=authentication/login-callback?state={stateId}", forceLoad: true);

            return new RemoteAuthenticationResult<RemoteAuthenticationState> { Status = RemoteAuthenticationStatus.Redirect };
        }

        public async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            var callbackUrl = context.Url;
            var stateId = EasyAuthRemoteAuthenticatorView.GetParameter(new Uri(callbackUrl).Query, "state");
            var serializedState = await JSRuntime.InvokeAsync<string>("sessionStorage.getItem", $"Blazor.EasyAuth.{stateId}");
            var state = JsonSerializer.Deserialize<RemoteAuthenticationState>(serializedState);

            return new RemoteAuthenticationResult<RemoteAuthenticationState> { State = state, Status = RemoteAuthenticationStatus.Success };
        }

        public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignOutAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            throw new NotImplementedException();
        }

        public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignOutAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
        {
            throw new NotImplementedException();
        }
    }
}
