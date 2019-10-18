using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppy.Configurations;
using VoidHuntersRevived.Client.Textures.Attributes;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Textures.Builders
{
    /// <summary>
    /// Simple class used to generate textures for ShipParts
    /// </summary>
    [IsBuilder(typeof(ShipPart))]
    class ShipPartBuilder : Builder
    {
        protected override Image Build(EntityConfiguration entity)
        {
            var config = entity.Data as ShipPartConfiguration;
            var image = new Bitmap(100, 100);
            var graphics = Graphics.FromImage(image);

            return image;
        }
    }
}
