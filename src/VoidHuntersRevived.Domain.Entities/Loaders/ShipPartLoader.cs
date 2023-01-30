using Guppy.Attributes;
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
        public void Load(IPackProvider packs)
        {
            var pack = packs.GetById(VoidHuntersPack.Id);

            pack.Add(new IResource[]
            {
                new ColorResource(ShipPartColors.Default, Color.Orange),
                new ShipPartResource(ShipParts.HullSquare)
                {
                    DrawConfiguration.Polygon(ShipPartColors.Hull, 4),
                    RigidConfiguration.Polygon(1f, 4),
                    JointableConfiguration.Polygon(4)
                },
                new ShipPartResource(ShipParts.HullTriangle)
                {
                    DrawConfiguration.Polygon(ShipPartColors.Hull, 3),
                    RigidConfiguration.Polygon(1f, 3),
                    JointableConfiguration.Polygon(3)
                }
            });
        }
    }
}
