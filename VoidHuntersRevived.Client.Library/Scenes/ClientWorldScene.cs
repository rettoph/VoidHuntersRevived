using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Layers;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class ClientWorldScene : WorldScene
    {
        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Layer 0: Default
            this.layers.Create<CameraLayer>(0, l =>
            {
                l.SetUpdateOrder(10);
                l.SetDrawOrder(20);
            });
            // Layer 1: Chunk
            this.layers.Create<CameraLayer>(1, l =>
            {
                l.SetUpdateOrder(20);
                l.SetDrawOrder(10);
            });
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.layers.TryDraw(gameTime);
        }
        #endregion
    }
}
