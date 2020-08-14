using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace AzureStaticWebApps.Blazor.Authentication
{
    public static class StaticWebAppsAuthenticationServiceCollectionExtensions
    {
        // TODO: Provide generic variants 
        public static IServiceCollection AddStaticWebAppsAuthentication(this IServiceCollection services)
        {
            services.AddRemoteAuthentication<RemoteAuthenticationState, RemoteUserAccount, EasyAuthOptions>(options =>
            {
                options.ProviderOptions.Providers.Add(new ExternalProvider("github", "GitHub"));
                options.ProviderOptions.Providers.Add(new ExternalProvider("twitter", "Twitter"));
                options.ProviderOptions.Providers.Add(new ExternalProvider("facebook", "Facebook"));
            });

            services.AddScoped<AuthenticationStateProvider, EasyAuthRemoteAuthenticationService>();

            return services;
        }
    }
}