using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
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
                    Shapes = new[]
                    {
                        new PolygonShape(
                            vertices: new Vertices()
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            },
                            density: 1f)
                    },
                    ConnectionNodes = new[]
                    {
                        new ConnectionNodeDto()
                        {
                            Position = new Vector2(0, 0.5f),
                            Rotation = MathHelper.PiOver2 * 0
                        },
                        new ConnectionNodeDto()
                        {
                            Position = new Vector2(0.5f, 0),
                            Rotation = MathHelper.PiOver2 * 1
                        },
                        new ConnectionNodeDto()
                        {
                            Position = new Vector2(1, 0.5f),
                            Rotation = MathHelper.PiOver2 * 2
                        },
                        new ConnectionNodeDto()
                        {
                            Position = new Vector2(0.5f, 1),
                            Rotation = MathHelper.PiOver2 * 3
                        }
                    }
                });
            });
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
