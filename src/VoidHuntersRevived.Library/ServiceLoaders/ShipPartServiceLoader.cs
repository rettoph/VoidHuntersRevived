using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using VoidHuntersRevived.Library.Contexts.Utilities;
using VoidHuntersRevived.Library.Dtos.Utilities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ShipPartServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            #region Services
            services.RegisterTypeFactory<ConnectionNode>(p => new ConnectionNode());

            services.RegisterTransient<ConnectionNode>();
            #endregion

            #region Entities
            services.RegisterTypeFactory<Hull>(p => new Hull());

            services.RegisterTransient(Constants.ServiceConfigurationKeys.ShipParts.Hull, typeof(Hull));
            #endregion

            services.RegisterSetup<ShipPartService>((shipParts, p, c) =>
            {
                shipParts.RegisterContext(new HullContext()
                {
                    Name = "ship-part:hull:square",
                    Centeroid = PolygonHelper.GetCenteroid(4),
                    Shapes = new[]
                    {
                        new PolygonShape(
                            vertices: PolygonHelper.GetVertices(4),
                            density: 1f)
                    },
                    ConnectionNodes = PolygonHelper.GetConnectionNodes(4)
                });

                shipParts.RegisterContext(new HullContext()
                {
                    Name = "ship-part:hull:triangle",
                    Centeroid = PolygonHelper.GetCenteroid(3),
                    Shapes = new[]
                    {
                        new PolygonShape(
                            vertices: PolygonHelper.GetVertices(3),
                            density: 1f)
                    },
                    ConnectionNodes = PolygonHelper.GetConnectionNodes(3)
                });
            });
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
