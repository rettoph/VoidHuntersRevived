using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Interfaces;
using VoidHuntersRevived.Server.Helpers;
using Lidgren.Network.Xna;
using VoidHuntersRevived.Networking.Groups;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities.Connections;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Server.Scenes
{
    /// <summary>
    /// Server implementation of the MainGameScene
    /// </summary>
    public class ServerMainGameScene : MainGameScene
    {
        #region Private Fields
        private ServerGroup _group;
        private Queue<INetworkEntity> _addedNetworkEntities;
        private Queue<INetworkEntity> _removedNetworkEntities;
        #endregion

        #region Constructors
        public ServerMainGameScene(
            IPeer peer,
            IServiceProvider provider,
            IGame game) : base(peer, provider, game)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Add default client specific message type handlers
            this.Group.MessageTypeHandlers.Add("update", this.HandleUpdateMessageType);

            // Save the server instance of the scenes group
            _group = this.Group as ServerGroup;

            // Create queue meant to contain new and removed entities
            _addedNetworkEntities = new Queue<INetworkEntity>();
            _removedNetworkEntities = new Queue<INetworkEntity>();

            // Add all event handlers
            this.NetworkEntities.OnAdded += this.HandleNetworkEntityAdded;
            this.NetworkEntities.OnRemove += this.HandleNetworkEntityRemoved;
            this.Group.Users.OnAdded += this.HandleUserAdded;
        }

        protected override void Initialize()
        {
            base.Initialize();

            var rand = new Random();
            for (Int32 i = 0; i < 50; i++)
            {
                var e = this.Entities.Create<ShipPart>("entity:hull:square");
                e.Body.Position = new Vector2((float)(rand.NextDouble() * 100) - 50, (float)(rand.NextDouble() * 100) - 50);
                e.Body.LinearVelocity = new Vector2((float)(rand.NextDouble() * 20) - 10, (float)(rand.NextDouble() * 20) - 10);

                e.Body.Rotation = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
                e.Body.AngularVelocity = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
            }

            for (Int32 i = 0; i < 50; i++)
            {
                var e = this.Entities.Create<ShipPart>("entity:hull:triangle");
                e.Body.Position = new Vector2((float)(rand.NextDouble() * 100) - 50, (float)(rand.NextDouble() * 100) - 50);
                e.Body.LinearVelocity = new Vector2((float)(rand.NextDouble() * 20) - 10, (float)(rand.NextDouble() * 20) - 10);

                e.Body.Rotation = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
                e.Body.AngularVelocity = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
            }

            for (Int32 i = 0; i < 150; i++)
            {
                var e = this.Entities.Create<ShipPart>("entity:thruster");
                e.Body.Position = new Vector2((float)(rand.NextDouble() * 100) - 50, (float)(rand.NextDouble() * 100) - 50);
                e.Body.LinearVelocity = new Vector2((float)(rand.NextDouble() * 20) - 10, (float)(rand.NextDouble() * 20) - 10);

                e.Body.Rotation = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
                e.Body.AngularVelocity = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
            }

            for (Int32 i = 0; i < 50; i++)
            {
                var e = this.Entities.Create<ShipPart>("entity:hull:beam");
                e.Body.Position = new Vector2((float)(rand.NextDouble() * 100) - 50, (float)(rand.NextDouble() * 100) - 50);
                e.Body.LinearVelocity = new Vector2((float)(rand.NextDouble() * 20) - 10, (float)(rand.NextDouble() * 20) - 10);

                e.Body.Rotation = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
                e.Body.AngularVelocity = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
            }

            /*
            for (Int32 i = 0; i < 50; i++)
            {
                var e = this.Entities.Create<ShipPart>("entity:hull:beam");
                e.Body.Position = new Vector2((float)(rand.NextDouble() * 100) - 50, (float)(rand.NextDouble() * 100) - 50);
                e.Body.LinearVelocity = new Vector2((float)(rand.NextDouble() * 20) - 10, (float)(rand.NextDouble() * 20) - 10);

                e.Body.Rotation = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
                e.Body.AngularVelocity = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
            }

            for (Int32 i = 0; i < 50; i++)
            {
                var e = this.Entities.Create<ShipPart>("entity:hull:triangle");
                e.Body.Position = new Vector2((float)(rand.NextDouble() * 100) - 50, (float)(rand.NextDouble() * 100) - 50);
                e.Body.LinearVelocity = new Vector2((float)(rand.NextDouble() * 20) - 10, (float)(rand.NextDouble() * 20) - 10);

                e.Body.Rotation = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
                e.Body.AngularVelocity = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
            }

            for (Int32 i = 0; i < 50; i++)
            {
                var e = this.Entities.Create<ShipPart>("entity:hull:hexagon");
                e.Body.Position = new Vector2((float)(rand.NextDouble() * 100) - 50, (float)(rand.NextDouble() * 100) - 50);
                e.Body.LinearVelocity = new Vector2((float)(rand.NextDouble() * 20) - 10, (float)(rand.NextDouble() * 20) - 10);

                e.Body.Rotation = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
                e.Body.AngularVelocity = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
            }

            for (Int32 i = 0; i < 150; i++)
            {
                var e = this.Entities.Create<ShipPart>("entity:thruster");
                e.Body.Position = new Vector2((float)(rand.NextDouble() * 100) - 50, (float)(rand.NextDouble() * 100) - 50);
                e.Body.LinearVelocity = new Vector2((float)(rand.NextDouble() * 20) - 10, (float)(rand.NextDouble() * 20) - 10);

                e.Body.Rotation = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
                e.Body.AngularVelocity = (float)(rand.NextDouble() * 6.28318530718) - 3.14159265359f;
            }
            */
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            // Push all new entities to all peers at this time
            while(_addedNetworkEntities.Count > 0)
                this.Group.SendMessage(
                ServerMessageHelper.BuildCreateNetworkEntityMessage(_addedNetworkEntities.Dequeue(), this.Group),
                NetDeliveryMethod.ReliableOrdered);

            // Push all removed entities to all peers at this time
            while (_removedNetworkEntities.Count > 0)
                this.Group.SendMessage(
                    ServerMessageHelper.BuildDestroyNetworkEntityMessage(_removedNetworkEntities.Dequeue(), this.Group),
                    NetDeliveryMethod.ReliableOrdered);

            base.Update(gameTime);
        }
        #endregion

        #region MessageType Handlers
        private void HandleUpdateMessageType(NetIncomingMessage im)
        {
            // Create a brand new entity from the server sent data...
            INetworkEntity entity = this.NetworkEntities.GetById(im.ReadInt64());
            entity.Read(im);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// The following method will handle any new incoming network entities and
        /// automatically add the entity to a message queue to be updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleNetworkEntityAdded(object sender, INetworkEntity e)
        {
            _addedNetworkEntities.Enqueue(e);        }

        /// <summary>
        /// The following method will handle any removed network entities and
        /// automatically add the entity to a message queue to be updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleNetworkEntityRemoved(object sender, INetworkEntity e)
        {
            _removedNetworkEntities.Enqueue(e);
        }

        /// <summary>
        /// When a new user joins we must update them with the current world state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="user"></param>
        private void HandleUserAdded(object sender, IUser user)
        {
            // Write & send a basic setup message, with global general message details
            var om = this.Group.CreateMessage("setup:begin");
            om.Write(this.World.Gravity);
            _group.SendMessage(om, user, NetDeliveryMethod.ReliableOrdered);

            // Send the new client every existing network entity within the scene
            foreach (INetworkEntity ne in this.NetworkEntities)
                _group.SendMessage(
                    ServerMessageHelper.BuildCreateNetworkEntityMessage(ne, this.Group),
                    user,
                    NetDeliveryMethod.ReliableOrdered);

            // Create a new UserPlayer instance for the new user..
            var bridge = this.Entities.Create<ShipPart>("entity:hull:triangle", null);
            this.Entities.Create<UserPlayer>("entity:player:user", null, user, bridge);

            bridge.Body.Rotation = 1f;

            om = this.Group.CreateMessage("setup:end");
            _group.SendMessage(om, user, NetDeliveryMethod.ReliableOrdered);
        }
        #endregion
    }
}
