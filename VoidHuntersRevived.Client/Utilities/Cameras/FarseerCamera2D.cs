using FarseerPhysics;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace VoidHuntersRevived.Client.Utilities.Cameras
{
    public class FarseerCamera2D : Camera2D
    {
        private Viewport _viewport;

        public FarseerCamera2D(GraphicsDevice graphics, GameWindow window) : base(graphics, window)
        {
            _viewport = graphics.Viewport;
            this.MoveBy(2f, 0);
        }

        protected override RectangleF buildViewportBounds()
        {
            return new RectangleF(
                x: 0,
                y: 0,
                width: ConvertUnits.ToSimUnits(_viewport.Bounds.Width),
                height: ConvertUnits.ToSimUnits(_viewport.Bounds.Height));
        }
    }
}
