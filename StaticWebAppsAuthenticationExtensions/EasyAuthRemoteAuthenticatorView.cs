using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StaticWebAppsAuthenticationExtensions
{
    public class EasyAuthRemoteAuthenticatorView : RemoteAuthenticatorView
    {
        [Parameter] public string SelectedOption { get; set; }

        [Inject] NavigationManager Navigation { get; set; }

        protected override Task OnParametersSetAsync()
        {
            switch (Action)
            {
                case RemoteAuthenticationActions.LogIn:
                    if (SelectedOption != null)
                    {
                        ProcessLogin();
                    }

                    return Task.CompletedTask;
                default:
                    return base.OnParametersSetAsync();
            }
        }

        private void ProcessLogin()
        {
            Navigation.NavigateTo($"/.auth/login/{SelectedOption}", forceLoad: true);
        }
    }
}
