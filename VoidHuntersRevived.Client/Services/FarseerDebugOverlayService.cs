using FarseerPhysics;
using FarseerPhysics.DebugView;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Client.Services
{
    /// <summary>
    /// A service used to interface directly with FarseerPhysics.MonoGame.DebigView
    /// and render the current world state as is...
    /// </summary>
    class FarseerDebugOverlayService : SceneObject, ISceneService
    {
        #region Private Fields
        private SpriteBatch _spriteBatch;
        private DebugViewXNA _debug;
        private ClientMainGameScene _scene;
        private GraphicsDevice _graphics;
        private ContentManager _content;
        #endregion

        #region Constructors
        public FarseerDebugOverlayService(GraphicsDevice graphics, ContentManager content, SpriteBatch spriteBatch, IGame game) : base(game)
        {
            _graphics = graphics;
            _content = content;
            _spriteBatch = spriteBatch;

            this.Enabled = true;
            this.Visible = true;
        }
        #endregion

        #region Initialization Methods
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
            _scene = this.Scene as ClientMainGameScene;
            _debug = new DebugViewXNA(_scene.World);
            _debug.LoadContent(_graphics, _content);

            _debug.AppendFlags(DebugViewFlags.ContactPoints);
            _debug.AppendFlags(DebugViewFlags.ContactNormals);
            //_debug.AppendFlags(DebugViewFlags.DebugPanel);
            //_debug.AppendFlags(DebugViewFlags.CenterOfMass);
            //_debug.AppendFlags(DebugViewFlags.Joint);
            //_debug.AppendFlags(DebugViewFlags.PerformanceGraph);
            //_debug.AppendFlags(DebugViewFlags.Shape);
            _debug.AppendFlags(DebugViewFlags.Controllers);

            this.SetVisible(false);
        }

        protected override void PreInitialize()
        {
            // throw new NotImplementedException();
        }
        #endregion

        #region Frame Methods
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
        #endregion
    }
}
