using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using tainicom.Aether.Physics2D;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Armors;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts.Weapons;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ShipPartServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            Settings.MaxPolygonVertices = 9;
            Settings.ContinuousPhysics = false;
            Directory.CreateDirectory(VHR.Directories.Resources.ShipParts);

            services.AddFactory<ShipPartService>(p => new ShipPartService());
            services.AddScoped<ShipPartService>();
            services.AddSetup<ShipPartService>((shipParts, p, c) =>
            {
                shipParts.ImportAll(VHR.Directories.Resources.ShipParts);
            });

            services.AddFactory<Hull>(p => new Hull());
            services.AddFactory<Thruster>(p => new Thruster());
            services.AddFactory<Gun>(p => new Gun());
            services.AddFactory<Laser>(p => new Laser());
            services.AddFactory<Armor>(b => new Armor());
            services.AddFactory<DroneBay>(b => new DroneBay());
            services.AddFactory<ShieldGenerator>(b => new ShieldGenerator());
            services.AddFactory<PowerCell>(b => new PowerCell());

            services.AddTransient<Hull>(VHR.Entities.Hull);
            services.AddTransient<Thruster>(VHR.Entities.Thruster);
            services.AddTransient<Gun>(VHR.Entities.Gun);
            services.AddTransient<Laser>(VHR.Entities.Laser);
            services.AddTransient<Armor>(VHR.Entities.Armor);
            services.AddTransient<DroneBay>(VHR.Entities.DroneBay);
            services.AddTransient<ShieldGenerator>(VHR.Entities.ShieldGenerator);
            services.AddTransient<PowerCell>(VHR.Entities.PowerCell);
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
