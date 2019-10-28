using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppy.Configurations;
using Guppy.Extensions.Collection;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Client.Textures.Attributes;
using VoidHuntersRevived.Client.Textures.Extensions.Farseer;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Color = System.Drawing.Color;

namespace VoidHuntersRevived.Client.Textures.TextureGenerators
{
    /// <summary>
    /// Simple class used to generate textures for ShipParts
    /// </summary>
    [IsTextureGenerator(typeof(ShipPart))]
    class ShipPartTextureGenerator : TextureGenerator
    {
        public static Pen Pen { get; set; } = new Pen(Color.FromArgb(255, 255, 255, 255), 2);
        public static Brush Brush { get; set; } = new SolidBrush(Color.FromArgb(200, 200, 200, 200));

        protected override Image Generate(EntityConfiguration entity)
        {
            var config = entity.Data as ShipPartConfiguration;
            return config.Vertices.ToImageDimensions().DrawShape(ShipPartTextureGenerator.Pen, ShipPartTextureGenerator.Brush);
        }
    }
}
