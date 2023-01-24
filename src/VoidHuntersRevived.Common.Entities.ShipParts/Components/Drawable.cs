using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public class Drawable
    {
        public readonly DrawConfiguration Configuration;
        public readonly Vector2 LocalCenter;
        public readonly Matrix LocalCenterTransformation;

        public Drawable(DrawConfiguration configuration)
        {
            this.Configuration = configuration;
            this.LocalCenter = configuration.Shapes.SelectMany(x => x).Average();
            this.LocalCenterTransformation = Matrix.CreateTranslation(this.LocalCenter.X, this.LocalCenter.Y, 0);
        }
    }
}
