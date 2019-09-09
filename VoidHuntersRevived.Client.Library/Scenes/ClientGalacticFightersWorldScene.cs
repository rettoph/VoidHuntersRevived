using FarseerPhysics;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using GalacticFighters.Client.Library.Utilities.Cameras;
using GalacticFighters.Library.Scenes;
using Guppy.Network.Peers;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Scenes
{
    internal sealed class ClientGalacticFightersWorldScene : GalacticFightersWorldScene
    {
        #region Private Fields
        private GraphicsDevice _graphics;
        private ContentManager _content;
        private DebugViewXNA _debugView;
        #endregion

        #region Public Fields
        public FarseerCamera2D Camera { get; private set; }
        #endregion

        #region Constructor
        public ClientGalacticFightersWorldScene(FarseerCamera2D camera, GraphicsDevice graphics, ContentManager content, World world) : base(world)
        {
            _graphics = graphics;
            _content = content;

            this.Camera = camera;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _debugView = new DebugViewXNA(this.world);
            _debugView.LoadContent(_graphics, _content);
            _debugView.AppendFlags(DebugViewFlags.ContactPoints);
            _debugView.AppendFlags(DebugViewFlags.ContactNormals);
            _debugView.AppendFlags(DebugViewFlags.Controllers);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Camera.TryUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _graphics.Clear(Color.Black);

            _debugView.RenderDebugData(this.Camera.Projection, this.Camera.View);
        }
        #endregion
    }
}
