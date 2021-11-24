using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Interfaces.Spells;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class SpellsServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, GuppyServiceCollection services)
        {
            services.RegisterTypeFactory<SpellService>(p => new SpellService());
            services.RegisterTypeFactory<FactoryServiceList<ISpell>>(p => new FactoryServiceList<ISpell>());

            services.RegisterScoped<SpellService>();
            services.RegisterScoped<FactoryServiceList<ISpell>>();
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
