using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Entities;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Client.Layers
{
    /// <summary>
    /// The main layer used for rendering objects within the MainGameScene.
    /// An instance of this class is used as the default layer when creating
    /// new entities within ClientMainGameScene
    /// </summary>
    public class MainGameLayer : Layer
    {
        #region Private Fields
        private ClientMainGameScene _scene;
        private SpriteBatch _spriteBatch;
        #endregion

        #region Constructors
        public MainGameLayer(SpriteBatch spriteBatch, IGame game) : base(game)
        {
            _spriteBatch = spriteBatch;

            this.SetVisible(true);
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            // throw new NotImplementedException();
        }

        protected override void PreInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            _scene = this.Scene as ClientMainGameScene;
        }

        protected override void PostInitialize()
        {
            // throw new NotImplementedException();
        }
        #endregion

        #region Frame Methods
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _scene.Camera.BasicEffect);

            this.Entities.Draw(gameTime);

            _spriteBatch.End();
        }
        #endregion
    }
}
