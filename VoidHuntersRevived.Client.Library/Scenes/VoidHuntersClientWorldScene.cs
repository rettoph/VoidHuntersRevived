﻿using FarseerPhysics;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Guppy.Network.Peers;
using Guppy.UI.Elements;
using Guppy.UI.Entities;
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
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class VoidHuntersClientWorldScene : VoidHuntersWorldScene
    {
        public World ServerWorld { get; private set; }

        private ClientPeer _client;
        private GraphicsDevice _graphics;
        private DebugViewXNA _debug;
        private DebugViewXNA _debugServer;
        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private FarseerCamera2D _camera;
        private Queue<NetIncomingMessage> _updateMessageQueue;
        private Queue<NetIncomingMessage> _actionMessageQueue;

        public VoidHuntersClientWorldScene(FarseerCamera2D camera, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager content, ClientPeer peer, World world, IServiceProvider provider) : base(peer, world, provider)
        {
            _camera = camera;
            _spriteBatch = spriteBatch;
            _graphics = graphicsDevice;
            _content = content;
            _client = peer;

            this.logger.LogInformation("Constructing!");
        }

        protected override void Boot()
        {
            this.logger.LogInformation("Booting!");

            base.Boot();

            this.ServerWorld = this.provider.GetRequiredService<World>();

            this.group.MessageHandler.Add("setup:begin", this.HandleSetupStartMessage);
            this.group.MessageHandler.Add("setup:end", this.HandleSetupEndMessage);
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            _debug = new DebugViewXNA(this.World);
            _debug.LoadContent(_graphics, _content);
            _debug.AppendFlags(DebugViewFlags.ContactPoints);
            _debug.AppendFlags(DebugViewFlags.ContactNormals);
            _debug.AppendFlags(DebugViewFlags.Controllers);

            _debugServer = new DebugViewXNA(this.ServerWorld);
            _debugServer.LoadContent(_graphics, _content);
            _debugServer.AppendFlags(DebugViewFlags.ContactPoints);
            _debugServer.AppendFlags(DebugViewFlags.ContactNormals);
            _debugServer.AppendFlags(DebugViewFlags.Controllers);
            _debugServer.DefaultShapeColor = Color.Blue;

            this.DefaultLayerDepth = 1;

            // this.layers.Create<HudLayer>(0, 0, 0, 1);
            this.layers.Create<CameraLayer>(1, 1, 0, 0);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.ServerWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
            _camera.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            _graphics.Clear(Color.Black);

            base.Draw(gameTime);

            _debug.RenderDebugData(_camera.Projection, _camera.View);
            _debugServer.RenderDebugData(_camera.Projection, _camera.View);

        }

        #region NetMessage Handlers
        private void HandleSetupStartMessage(NetIncomingMessage obj)
        {
            _updateMessageQueue = new Queue<NetIncomingMessage>();
            _actionMessageQueue = new Queue<NetIncomingMessage>();

            this.group.MessageHandler["action"] = this.EnqueueActionMessage;
            this.group.MessageHandler["update"] = this.EnqueueUpdateMessage;
            this.group.MessageHandler["create"] = this.HandleCreateMessage;
            
        }
        private void HandleSetupEndMessage(NetIncomingMessage obj)
        {
            this.group.MessageHandler["action"] = this.HandleActionMessage;
            this.group.MessageHandler["update"] = this.HandleUpdateMessage;

            // Flush the collected queue while the client was settingup
            while (_updateMessageQueue.Count > 0)
                this.HandleUpdateMessage(_updateMessageQueue.Dequeue());
            while (_actionMessageQueue.Count > 0)
                this.HandleActionMessage(_actionMessageQueue.Dequeue());

            // Empty the message queue
            _updateMessageQueue.Clear();
            _actionMessageQueue.Clear();
        }

        private void HandleCreateMessage(NetIncomingMessage obj)
        {
            var ne = this.entities.Create<NetworkEntity>(obj.ReadString(), obj.ReadGuid());
            ne.Read(obj);
        }
        private void HandleUpdateMessage(NetIncomingMessage obj)
        {
            var ne = this.networkEntities.GetById(obj.ReadGuid());
            ne.Read(obj);

            // Mark the entity as clean now
            ne.Dirty = false;
        }
        private void HandleActionMessage(NetIncomingMessage obj)
        {
            this.networkEntities.GetById(obj.ReadGuid()).HandleAction(obj.ReadString(), obj);
        }

        /// <summary>
        /// Special message queues used to hold group methods
        /// recieved before setup is complete.
        /// </summary>
        /// <param name="obj"></param>
        private void EnqueueUpdateMessage(NetIncomingMessage obj)
        {
            _updateMessageQueue.Enqueue(obj);
        }
        private void EnqueueActionMessage(NetIncomingMessage obj)
        {
            _actionMessageQueue.Enqueue(obj);
        }
        #endregion
    }
}
