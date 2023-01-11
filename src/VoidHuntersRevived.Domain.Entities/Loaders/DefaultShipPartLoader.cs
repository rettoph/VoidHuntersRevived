using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    public class DefaultShipPartLoader : IShipPartConfigurationLoader
    {
        public void Load(IShipPartConfigurationService shipParts)
        {
            shipParts.Add(new ShipPartConfiguration("hull:square")
            {
                new Drawn()
                {
                    Shapes = new[]
                    {
                        new[]
                        {
                            new Vector2(0, 0),
                            new Vector2(1, 0),
                            new Vector2(1, 1),
                            new Vector2(0, 1)
                        }
                    }
                },
                new Rigid()
                {
                    Shapes = new[]
                    {
                        new PolygonShape(
                            vertices: new Vertices()
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(0, 1)
                            },
                            density: 1f)
                    }
                }
            });
        }
    }
}
