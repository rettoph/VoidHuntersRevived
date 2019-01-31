using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Scenes.Interfaces;
using VoidHuntersRevived.Networking.Interfaces;
using VoidHuntersRevived.Networking.Scenes;

namespace VoidHuntersRevived.Library.Scenes
{
    /// <summary>
    /// The main scene will manage actual gameplay within the game
    /// </summary>
    public class MainScene : NetworkScene, IFarseerScene
    {
        public World World { get; set; }
        public IGroup Group { get; set; }

        private INetworkEntity _dirtyEntity;
        private NetOutgoingMessage _om;

        private DateTime _lastMessagePush;
        protected Int32 MessagePushInterval = 0;

        public Int32 MessageCount { get; private set; }


        public MainScene(IServiceProvider provider, IGame game) : base(provider, game)
        {
            this.Enabled = true;

            _lastMessagePush = DateTime.Now;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create a new farseer world
            this.World = new World(Vector2.Zero);
        }

        public override void Update(GameTime gameTime)
        {
            this.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000); // Step world forward first

            base.Update(gameTime);

            if (DateTime.Now.Subtract(_lastMessagePush).TotalMilliseconds >= this.MessagePushInterval)
            {
                if (this.NetworkEntities.DirtyNetworkEntityQueue.Count > 0)
                {
                    this.MessageCount = this.NetworkEntities.DirtyNetworkEntityQueue.Count;

                    while (this.NetworkEntities.DirtyNetworkEntityQueue.Count > 0)
                    {
                        // Select the dirty entity
                        _dirtyEntity = this.NetworkEntities.DirtyNetworkEntityQueue.Dequeue();

                        // Create a new message and send it to the group
                        _om = this.Group.CreateMessage("update");
                        _dirtyEntity.Write(_om);
                        this.Group.SendMessage(_om, NetDeliveryMethod.UnreliableSequenced, 0);

                        // Mark the entity as clean
                        _dirtyEntity.Dirty = false;
                    }

                    _lastMessagePush = DateTime.Now;
                }
            }
        }
    }
}
