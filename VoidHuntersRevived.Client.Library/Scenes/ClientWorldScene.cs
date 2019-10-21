using FarseerPhysics;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Layers;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Scenes;
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

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public sealed class ClientWorldScene : WorldScene
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
        private BasicEffect _effect;

        private Boolean _wasDebugDown;
        private Boolean _debug;
        #endregion

        #region Public Fields
        public FarseerCamera2D Camera { get; private set; }
        public Sensor Sensor { get; private set; }
        /// <summary>
        /// The scale value used internally by chunks
        /// </summary>
        public Single ChunkScale { get; private set; }
        #endregion

        #region Constructor
        public ClientWorldScene(SpriteBatch spriteBatch, ServerRender server, FarseerCamera2D camera, GraphicsDevice graphics, ContentManager content, World world) : base(world)
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

            _effect = new BasicEffect(_graphics)
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };

            this.layers.Create<DebugLayer>(100);

            this.Sensor = this.entities.Create<Sensor>("sensor");
            this.debugOverlay = this.entities.Create<DebugOverlay>("debug-overlay");

            this.layers.Create<BackgroundLayer>(99);
            this.layers.Create<WorldLayer>(0);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _debugView = new DebugViewXNA(this.world);
            _debugView.LoadContent(_graphics, _content);
            _debugView.RemoveFlags(DebugViewFlags.Joint);

            _serverDebugView = new DebugViewXNA(_server.World);
            _serverDebugView.LoadContent(_graphics, _content);
            _serverDebugView.AppendFlags(DebugViewFlags.ContactPoints);
            _serverDebugView.AppendFlags(DebugViewFlags.ContactNormals);
            _serverDebugView.AppendFlags(DebugViewFlags.Controllers);
            _serverDebugView.SleepingShapeColor = Color.Red;
            _serverDebugView.DefaultShapeColor = Color.Blue;

            this.debugOverlay.AddLine(() => $"Actions: {this.ActionsRecieved.ToString("#,##0")}");
            this.debugOverlay.AddLine(() => $"APS: {this.ActionsPerSecond.ToString("#0.##0")}");
            this.debugOverlay.AddLine(() => $"FPS: {this.FramesPerSecond.ToString("#,##0")}");
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            // Update the internal chunk scale value, used locally by client chunks for rendering
            var inverse = Math.Ceiling(1 / this.Camera.ZoomTarget);
            this.ChunkScale = MathHelper.Clamp((Single)Math.Round((1 / (inverse + (inverse % 2)) * 2), 3), 0.125f, 1);

            base.Update(gameTime);

            this.entities.TryUpdate(gameTime);
            this.layers.TryUpdate(gameTime);
            this.Camera.TryUpdate(gameTime);

            var debug = Keyboard.GetState().IsKeyDown(Keys.F1);

            if(debug && !_wasDebugDown)
                _debug = !_debug;

            _wasDebugDown = debug;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _graphics.Clear(Color.Black);

            // Update the internal effect
            _effect.Projection = this.Camera.Projection;
            _effect.View = this.Camera.View;

            // Draw all entities
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.LinearClamp, effect: _effect);
            this.chunks.TryDraw(gameTime);
            _spriteBatch.End();

            this.layers.TryDraw(gameTime);

            if (_debug)
            {
                _serverDebugView.RenderDebugData(this.Camera.Projection, this.Camera.View);
                _debugView.RenderDebugData(this.Camera.Projection, this.Camera.View);
            }
        }
        #endregion
    }
}
