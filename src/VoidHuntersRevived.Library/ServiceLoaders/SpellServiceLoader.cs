using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Drivers.Services.Spells;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Services.SpellCasts;
using VoidHuntersRevived.Library.Services.Spells;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    /// <summary>
    /// Default service loader responsible for 
    /// initializing all spells & spellcasts.
    /// </summary>
    [AutoLoad]
    internal sealed class SpellServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            #region Spell Services 
            // Factories...
            services.AddFactory<SpellCastService>(p => new SpellCastService());
            services.AddFactory<FrameableList<Spell>>(p => new FrameableList<Spell>());

            //Services...
            services.AddScoped<SpellCastService>();
            services.AddScoped<FrameableList<Spell>>();
            #endregion

            #region SpellCasts
            // Factories...
            services.AddFactory<LaunchDroneSpellCast>(p => new LaunchDroneSpellCast());
            
            // Services...
            services.AddScoped<LaunchDroneSpellCast>();
            #endregion

            #region Spells
            // Factories...
            services.AddFactory<LaunchDroneSpell>(p => new LaunchDroneSpell());
            
            // Services...
            services.AddTransient<LaunchDroneSpell>();

            // Drivers
            services.AddAndBindDriver<LaunchDroneSpell, LaunchDroneSpellMasterNetworkAuthorizationDriver>(p => new LaunchDroneSpellMasterNetworkAuthorizationDriver());
            #endregion
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
