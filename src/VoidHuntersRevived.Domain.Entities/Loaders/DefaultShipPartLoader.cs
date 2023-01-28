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
                DrawConfiguration.Polygon(ShipPartColors.Hull, 4),
                RigidConfiguration.Polygon(1f, 4),
                JointableConfiguration.Polygon(4)
            });

            shipParts.Add(new ShipPartConfiguration(ShipParts.HullTriangle)
            {
                DrawConfiguration.Polygon(ShipPartColors.Hull, 3),
                RigidConfiguration.Polygon(1f, 3),
                JointableConfiguration.Polygon(3)
            });
        }
    }
}
