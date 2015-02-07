﻿using Owin;
using SelfHost.Config;
using System.Collections.Generic;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services.InMemory;
using Thinktecture.IdentityServer.Host.Config;
using Thinktecture.IdentityServer.WsFederation.Configuration;
using Thinktecture.IdentityServer.WsFederation.Models;
using Thinktecture.IdentityServer.WsFederation.Services;

namespace SelfHost
{
    internal class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var factory = InMemoryFactory.Create(
                users:   Users.Get(), 
                clients: Clients.Get(), 
                scopes:  Scopes.Get());

            var options = new IdentityServerOptions
            {
                IssuerUri = "https://idsrv3.com",
                SiteName = "Thinktecture IdentityServer3 - WsFed",

                SigningCertificate = Certificate.Get(),
                Factory = factory,
                PluginConfiguration = ConfigurePlugins,
            };

            appBuilder.UseIdentityServer(options);
        }

        private void ConfigurePlugins(IAppBuilder pluginApp, IdentityServerOptions options)
        {
            var wsFedOptions = new WsFederationPluginOptions(options);

            // data sources for in-memory services
            wsFedOptions.Factory.Register(new Registration<IEnumerable<RelyingParty>>(RelyingParties.Get()));
            wsFedOptions.Factory.RelyingPartyService = new Registration<IRelyingPartyService>(typeof(InMemoryRelyingPartyService));

            pluginApp.UseWsFederationPlugin(wsFedOptions);
        }
    }
}