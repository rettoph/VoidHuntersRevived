using FarseerPhysics;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GalacticFighters.Client.Library.Utilities.Cameras
{
    public sealed class FarseerCamera2D : Camera2D
    {
        private GraphicsDevice _graphics;

        public FarseerCamera2D(GraphicsDevice graphics, GameWindow window) : base(graphics, window)
        {
            _graphics = graphics;
        }

        protected override RectangleF buildViewportBounds()
        {
            return new RectangleF(
                x: 0,
                y: 0,
                width: ConvertUnits.ToSimUnits(_graphics.Viewport.Bounds.Width),
                height: ConvertUnits.ToSimUnits(_graphics.Viewport.Bounds.Height));
        }
    }
}
