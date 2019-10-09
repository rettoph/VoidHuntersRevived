using FarseerPhysics;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using GalacticFighters.Client.Library.Entities;
using GalacticFighters.Client.Library.Layers;
using GalacticFighters.Client.Library.Utilities;
using GalacticFighters.Client.Library.Utilities.Cameras;
using GalacticFighters.Library.Scenes;
using Guppy.Collections;
using Guppy.Network.Peers;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticFighters.Client.Library.Scenes
{
    public sealed class ClientGalacticFightersWorldScene : GalacticFightersWorldScene
    {
        #region Internal Fields
        internal DebugOverlay debugOverlay;
        #endregion

        #region Private Fields
        private GraphicsDevice _graphics;
        private ContentManager _content;
        private DebugViewXNA _debugView;
        private DebugViewXNA _serverDebugView;
        private ServerRender _server;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        private Boolean _wasDebugDown;
        private Boolean _debug;
        #endregion

        #region Public Fields
        public FarseerCamera2D Camera { get; private set; }
        public Sensor Sensor { get; private set; }
        #endregion

        #region Constructor
        public ClientGalacticFightersWorldScene(SpriteBatch spriteBatch, ServerRender server, FarseerCamera2D camera, GraphicsDevice graphics, ContentManager content, World world) : base(world)
        {
            _spriteBatch = spriteBatch;
            _server = server;
            _graphics = graphics;
            _content = content;
            _font = content.Load<SpriteFont>("font");

            this.Camera = camera;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.layers.Create<DebugLayer>(1);

            this.Sensor = this.entities.Create<Sensor>("sensor");
            this.debugOverlay = this.entities.Create<DebugOverlay>("debug-overlay");

            this.layers.Create<WorldLayer>(0);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _debugView = new DebugViewXNA(this.world);
            _debugView.LoadContent(_graphics, _content);
            _debugView.AppendFlags(DebugViewFlags.ContactPoints);
            _debugView.AppendFlags(DebugViewFlags.ContactNormals);
            _debugView.AppendFlags(DebugViewFlags.Controllers);

            _serverDebugView = new DebugViewXNA(_server.World);
            _serverDebugView.LoadContent(_graphics, _content);
            _serverDebugView.AppendFlags(DebugViewFlags.ContactPoints);
            _serverDebugView.AppendFlags(DebugViewFlags.ContactNormals);
            _serverDebugView.AppendFlags(DebugViewFlags.Controllers);
            _serverDebugView.SleepingShapeColor = Color.Red;
            _serverDebugView.DefaultShapeColor = Color.Blue;

            this.debugOverlay.AddLine(() => $"Actions: {this.ActionsRecieved.ToString("#,##0")}");
            this.debugOverlay.AddLine(() => $"Actions Per Second: {this.ActionsPerSecond.ToString("#0.##0")}");
            this.debugOverlay.AddLine(() => $"FPS: {this.FramesPerSecond.ToString("#,##0")}");
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Camera.TryUpdate(gameTime);
            this.entities.TryUpdate(gameTime);
            this.layers.TryUpdate(gameTime);

            var debug = Keyboard.GetState().IsKeyDown(Keys.F1);

            if(debug && !_wasDebugDown)
                _debug = !_debug;

            _wasDebugDown = debug;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _graphics.Clear(Color.Black);

            if(_debug)
                _serverDebugView.RenderDebugData(this.Camera.Projection, this.Camera.View);
            _debugView.RenderDebugData(this.Camera.Projection, this.Camera.View);

            this.layers.TryDraw(gameTime);
        }
        #endregion
    }
}
