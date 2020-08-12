using System.Collections;
using System.Collections.Generic;

namespace AzureStaticWebApps.Blazor.Authentication
{
    public class EasyAuthOptions
    {
        public IList<ExternalProvider> Providers { get; set; } = new List<ExternalProvider>();
        public string AuthenticationDataUrl { get; set; }
    }

    public class ExternalProvider
    {
        public ExternalProvider(string id, string name)
        {
            Id = id;
            DisplayName = name;
        }

        public string Id { get; set; }
        public string DisplayName { get; set; }
    }
}