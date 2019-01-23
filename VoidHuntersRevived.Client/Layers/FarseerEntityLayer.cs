using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Client.Layers
{
    /// <summary>
    /// A layer dedicated to rendering farseer entities.
    /// It will assume that the scene is a ClientMainScene
    /// </summary>
    public class FarseerEntityLayer : Layer
    {
        private SpriteBatch _spriteBatch;
        private MainSceneClient _scene;

        public FarseerEntityLayer(SpriteBatch spriteBatch, IGame game) : base(game)
        {
            _spriteBatch = spriteBatch;

            this.Visible = true;
            this.Enabled = true;
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _scene.Camera.BasicEffect);

            this.Entities.Draw(gameTime);

            _spriteBatch.End();
        }

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
            _scene = this.Scene as MainSceneClient;
        }

        protected override void PostInitialize()
        {
            // throw new NotImplementedException();
        }
    }
}
