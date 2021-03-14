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
using VoidHuntersRevived.Library.Services.SpellCasts.AmmunitionSpellCasts;
using VoidHuntersRevived.Library.Services.Spells;
using VoidHuntersRevived.Library.Services.Spells.AmmunitionSpells;

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
            services.AddFactory<OrderableList<Spell>>(p => new OrderableList<Spell>());

            //Services...
            services.AddScoped<SpellCastService>();
            services.AddScoped<OrderableList<Spell>>();
            #endregion

            #region SpellCasts
            // Factories...
            services.AddFactory<LaunchDroneSpellCast>(p => new LaunchDroneSpellCast());
            services.AddFactory<BulletSpellCast>(p => new BulletSpellCast());
            services.AddFactory<LaserBeamSpellCast>(p => new LaserBeamSpellCast());
            
            // Services...
            services.AddScoped<LaunchDroneSpellCast>();
            services.AddScoped<BulletSpellCast>();
            services.AddScoped<LaserBeamSpellCast>();
            #endregion

            #region Spells
            // Factories...
            services.AddFactory<LaunchDroneSpell>(p => new LaunchDroneSpell());
            services.AddFactory<BulletSpell>(p => new BulletSpell());
            services.AddFactory<LaserBeamSpell>(p => new LaserBeamSpell());
            
            // Services...
            services.AddTransient<LaunchDroneSpell>();
            services.AddTransient<BulletSpell>();
            services.AddTransient<LaserBeamSpell>();

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
