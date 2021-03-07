using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.Extensions.System.Collections;
using Guppy.Interfaces;
using Guppy.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Drivers.Entities.Spells;
using VoidHuntersRevived.Library.Entities.Spells;
using VoidHuntersRevived.Library.Services;
using ServiceCollection = Guppy.DependencyInjection.ServiceCollection;
using ServiceProvider = Guppy.DependencyInjection.ServiceProvider;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class SpellsServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            // Services
            services.AddFactory<SpellService>(p => new SpellService());
            services.AddScoped<SpellService>();

            // Spell Factories
            services.AddFactory<LaunchDroneSpell>(p => new LaunchDroneSpell());

            // Spells
            services.AddTransient<LaunchDroneSpell>();

            // Spell Drivers
            services.AddAndBindDriver<LaunchDroneSpell, LaunchDroneSpellMasterNetworkAuthorizationDriver>(p => new LaunchDroneSpellMasterNetworkAuthorizationDriver());
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
