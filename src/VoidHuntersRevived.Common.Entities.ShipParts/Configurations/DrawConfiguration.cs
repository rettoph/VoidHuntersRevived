using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Configurations
{
    public sealed class DrawConfiguration : IShipPartComponentConfiguration
    {
        private readonly Drawable _component;

        public readonly string Color;
        public readonly Vector2[][] Shapes;
        public readonly Vector2[][] Paths;

        public DrawConfiguration(string color, Vector2[][] shapes, Vector2[][] paths)
        {
            _component = new Drawable(this);

            this.Color = color;
            this.Shapes = shapes;
            this.Paths = paths;
        }

        public void AttachComponentTo(Entity entity)
        {
            entity.Attach(_component);
        }
    }
}
