using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Pooling.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.Utilities.Drawing;
using VoidHuntersRevived.Library.Entities.Ammunitions;

namespace VoidHuntersRevived.Client.Library.Layers
{
    public class BackgroundLayer : WorldLayer
    {
        #region Constructor
        public BackgroundLayer(IPool<Projectile> projectiles, DebugOverlay debug, GraphicsDevice graphics, SpriteBatch spriteBatch, ClientWorldScene scene) : base(projectiles, debug, graphics, spriteBatch, scene)
        {
        }
        #endregion

        #region Life Cycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.SetDrawOrder(0);
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        #endregion
    }
}
