using Guppy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Layers
{
    public class BasicLayer : Layer
    {
        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Entities.TryUpdate(gameTime);
        }
        #endregion
    }
}
