using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    public class DefaultShipPartLoader : IShipPartConfigurationLoader
    {
        public void Load(IShipPartConfigurationService shipParts)
        {
            shipParts.Add(new ShipPartConfiguration(ShipParts.HullSquare)
            {
                new DrawConfiguration(
                    color: ShipPartColors.Hull,
                    shapes: new[]
                    {
                        new[]
                        {
                            new Vector2(0, 0),
                            new Vector2(1, 0),
                            new Vector2(1, 1),
                            new Vector2(0, 1)
                        }
                    },
                    paths: new[]
                    {
                        new[]
                        {
                            new Vector2(0, 0),
                            new Vector2(1, 0),
                            new Vector2(1, 1),
                            new Vector2(0, 1),
                            new Vector2(0, 0)
                        }
                    }),
                new RigidConfiguration(
                    shapes: new[]
                    {
                        new PolygonShape(
                            vertices: GiftWrap.GetConvexHull(new Vertices()
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(0, 1)
                            }),
                            density: 1f)
                    }),
                new JointableConfiguration(
                    joints: new[]
                    {
                        new JointConfiguration(
                            position: new Vector2(1f, 0.5f),
                            rotation: MathHelper.PiOver2 * 0),
                        new JointConfiguration(
                            position: new Vector2(0.5f, 1),
                            rotation: MathHelper.PiOver2 * 1),
                        new JointConfiguration(
                            position: new Vector2(0f, 0.5f),
                            rotation: MathHelper.PiOver2 * 2),
                        new JointConfiguration(
                            position: new Vector2(0.5f, 0f),
                            rotation: MathHelper.PiOver2 * 3)
                    })
            });
        }
    }
}
