using FarseerPhysics;
using Guppy.DependencyInjection;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Client.Library.Utilities.Cameras
{
    public class FarseerCamera2D : Camera2D
    {
        private GraphicsDevice _graphics;

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _graphics);
        }

        protected override RectangleF buildViewportBounds()
            => new RectangleF(
                x: 0,
                y: 0,
                width: ConvertUnits.ToSimUnits(_graphics.Viewport.Bounds.Width),
                height: ConvertUnits.ToSimUnits(_graphics.Viewport.Bounds.Height));
    }
}
