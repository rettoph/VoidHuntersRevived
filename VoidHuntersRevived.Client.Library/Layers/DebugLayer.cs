using Guppy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Layers
{
    public class DebugLayer : Layer
    {
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.entities.TryDraw(gameTime);
        }
    }
}
