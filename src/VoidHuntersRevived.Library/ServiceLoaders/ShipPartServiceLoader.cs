using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using tainicom.Aether.Physics2D;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Entities.ShipParts;
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
            Directory.CreateDirectory("ShipParts");

            services.AddFactory<ShipPartService>(p => new ShipPartService());
            services.AddScoped<ShipPartService>();
            services.AddSetup<ShipPartService>((shipParts, p, c) =>
            {
                shipParts.Import("ShipParts");
            });

            services.AddFactory<RigidShipPart>(p => new RigidShipPart());
            services.AddTransient<RigidShipPart>("entity:ship-part:rigid-ship-part");

            var part = new RigidShipPartContext("hull:pentagon");
            part.Shapes.TryAdd(builder =>
            {
                builder.AddSide(MathHelper.ToRadians(0));
                builder.AddSide(MathHelper.ToRadians(90));
                builder.AddSide(MathHelper.ToRadians(150));
                builder.AddSide(MathHelper.ToRadians(60));
                builder.AddSide(MathHelper.ToRadians(150));
            });
            part.Export("ShipParts/hull_pentagon.vhsp");
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
