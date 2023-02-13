using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain
{
    internal sealed class Screen : IScreen
    {
        public Camera2D Camera { get; }
        public GameWindow Window { get; }

        public Screen(GraphicsDevice graphics, GameWindow window)
        {
            this.Window = window;
            this.Camera = new Camera2D(graphics, window)
            {
                Center = false,
                Zoom = 1
            };
        }
    }
}
