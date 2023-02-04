using Guppy.Attributes;
using Guppy.Attributes.Common;
using Guppy.Loaders;
using Guppy.MonoGame.Resources;
using Guppy.Resources;
using Guppy.Resources.Definitions;
using Guppy.Resources.Loaders;
using Guppy.Resources.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    public class ShipPartLoader : IPackLoader
    {
        const float DefaultDensity = 1.0f;
        const int TriangleSides = 3;
        const int SquareSides = 4;
        const int HexagonSides = 6;

        public void Load(IPackProvider packs)
        {
            var pack = packs.GetById(VoidHuntersPack.Id);

            pack.Add(new IResource[]
            {
                new ShipPartResource(ShipParts.HullTriangle)
                {
                    DrawConfiguration.Polygon(Colors.Orange, TriangleSides),
                    RigidConfiguration.Polygon(DefaultDensity, TriangleSides),
                    JointableConfiguration.Polygon(TriangleSides)
                },
                new ShipPartResource(ShipParts.HullSquare)
                {
                    DrawConfiguration.Polygon(Colors.Orange, SquareSides),
                    RigidConfiguration.Polygon(DefaultDensity, SquareSides),
                    JointableConfiguration.Polygon(SquareSides)
                },
                new ShipPartResource(ShipParts.HullHexagon)
                {
                    DrawConfiguration.Polygon(Colors.Orange, HexagonSides),
                    RigidConfiguration.Polygon(DefaultDensity, HexagonSides),
                    JointableConfiguration.Polygon(HexagonSides)
                }
            });
        }
    }
}
