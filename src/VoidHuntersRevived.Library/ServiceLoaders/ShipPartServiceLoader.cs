using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using tainicom.Aether.Physics2D;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Armors;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ShipPartServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            Settings.MaxPolygonVertices = 9;
            Settings.ContinuousPhysics = false;
            Directory.CreateDirectory("Resources/ShipParts");
            String DefaultShipPartLocation = "Resources/ShipParts";

            services.AddFactory<ShipPartService>(p => new ShipPartService());
            services.AddScoped<ShipPartService>();
            services.AddSetup<ShipPartService>((shipParts, p, c) =>
            {
                shipParts.ImportAll(DefaultShipPartLocation);
            });

            services.AddFactory<RigidShipPart>(p => new RigidShipPart());
            services.AddFactory<Thruster>(p => new Thruster());
            services.AddFactory<Gun>(p => new Gun());
            services.AddFactory<Bullet>(b => new Bullet());
            services.AddFactory<Armor>(b => new Armor());
            services.AddTransient<RigidShipPart>(VHR.Entities.RigidShipPart);
            services.AddTransient<Thruster>(VHR.Entities.Thruster);
            services.AddTransient<Gun>(VHR.Entities.Gun);
            services.AddTransient<Bullet>();
            services.AddTransient<Armor>(VHR.Entities.Armor);
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
