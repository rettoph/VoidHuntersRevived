using FarseerPhysics;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Guppy.Network.Peers;
using Guppy.UI.Elements;
using Guppy.UI.Entities;
using Guppy.UI.Utilities.Units;
using Guppy.UI.Utilities.Units.UnitValues;
using Guppy.Utilities.Cameras;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Layers;
using VoidHuntersRevived.Client.Library.UI;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class VoidHuntersClientWorldScene : VoidHuntersWorldScene
    {
        public ChatWindow Chat { get; private set; }

        private ServerRender _server;
        private ClientPeer _client;
        private GraphicsDevice _graphics;
        private DebugViewXNA _debug;
        private DebugViewXNA _debugServer;
        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private FarseerCamera2D _camera;

        public VoidHuntersClientWorldScene(FarseerCamera2D camera, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager content, ClientPeer peer, World world, IServiceProvider provider) : base(peer, world, provider)
        {
            _camera = camera;
            _spriteBatch = spriteBatch;
            _graphics = graphicsDevice;
            _content = content;
            _client = peer;
        }

        protected override void Boot()
        {
            base.Boot();

            this.logger.LogInformation("Booting!");

            _server = this.provider.GetRequiredService<ServerRender>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            _debug = new DebugViewXNA(this.provider.GetRequiredService<World>());
            _debug.LoadContent(_graphics, _content);
            _debug.AppendFlags(DebugViewFlags.ContactPoints);
            _debug.AppendFlags(DebugViewFlags.ContactNormals);
            _debug.AppendFlags(DebugViewFlags.Controllers);

            _debugServer = new DebugViewXNA(_server.World);
            _debugServer.LoadContent(_graphics, _content);
            _debugServer.AppendFlags(DebugViewFlags.ContactPoints);
            _debugServer.AppendFlags(DebugViewFlags.ContactNormals);
            _debugServer.AppendFlags(DebugViewFlags.Controllers);
            _debugServer.DefaultShapeColor = new Color(0, 0, 255, 100);
            _debugServer.SleepingShapeColor = new Color(255, 0, 0, 100);

            this.DefaultLayerDepth = 1;

            this.layers.Create<HudLayer>(0, 0, 0, 200);
            this.layers.Create<CameraLayer>(1, 1, 0, 100);
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            var stage = this.entities.Create<Stage>("ui:stage", 0);
            this.Chat = stage.Content.CreateElement<ChatWindow>(0, new UnitValue[] { 1f, -150 }, 450, 150, this.Group);
        }

        protected override void update(GameTime gameTime)
        {
            base.update(gameTime);

            _camera.Update(gameTime);
        }
        protected override void draw(GameTime gameTime)
        {
            _graphics.Clear(Color.Black);

            // _debugServer.RenderDebugData(_camera.Projection, _camera.View);
            _debug.RenderDebugData(_camera.Projection, _camera.View);
            

            base.draw(gameTime);
        }
    }
}
