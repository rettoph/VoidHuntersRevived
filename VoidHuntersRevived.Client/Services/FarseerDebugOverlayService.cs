using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.DebugView;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Client.Services
{
    class FarseerDebugOverlayService : SceneObject, ISceneService
    {
        private SpriteBatch _spriteBatch;
        private DebugViewXNA _debug;
        private MainSceneClient _scene;
        private GraphicsDevice _graphics;
        private ContentManager _content;

        public FarseerDebugOverlayService(GraphicsDevice graphics, ContentManager content, SpriteBatch spriteBatch, IGame game) : base(game)
        {
            _graphics = graphics;
            _content = content;
            _spriteBatch = spriteBatch;

            this.Enabled = true;
            this.Visible = true;
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _debug.RenderDebugData(_scene.Camera.Projection, Matrix.Identity);

            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        protected override void Boot()
        {
            // throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PostInitialize()
        {
            _scene = this.Scene as MainSceneClient;
            _debug = new DebugViewXNA(_scene.World);
            _debug.LoadContent(_graphics, _content);
        }

        protected override void PreInitialize()
        {
            // throw new NotImplementedException();
        }
    }
}
