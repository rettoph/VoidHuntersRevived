using Guppy.UI.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Layers;

namespace VoidHuntersRevived.Client.Library.Layers
{
    public class PrimitiveLayer : CameraLayer
    {
        #region Protected Fields
        protected PrimitiveBatch primitiveBatch;
        #endregion

        #region Constructor
        public PrimitiveLayer(PrimitiveBatch primitiveBatch, SpriteBatch spriteBatch, BasicEffect effect, FarseerCamera2D camera) : base(spriteBatch, effect, camera)
        {
            this.primitiveBatch = primitiveBatch;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            this.primitiveBatch.Begin(
                this.camera.View, 
                this.camera.Projection, 
                BlendState.AlphaBlend);
            base.Draw(gameTime);
            this.primitiveBatch.End();
        }
        #endregion
    }
}
