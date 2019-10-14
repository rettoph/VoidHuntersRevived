using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Scenes;
using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Extensions.Collection;
using Guppy.Network.Security;
using Microsoft.Xna.Framework;
using System.Linq;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Server.Drivers
{
    [IsDriver(typeof(NetworkScene))]
    internal class ServerNetworkSceneDriver : Driver<NetworkScene>
    {
        #region Private Fields
        private EntityCollection _entities;
        private Queue<User> _newUsers;
        private Queue<NetworkEntity> _dirty;
        private Interval _interval;
        private NetPeer _peer;
        #endregion

        #region Constructor
        public ServerNetworkSceneDriver(Interval interval, NetPeer peer, EntityCollection entities, NetworkScene driven) : base(driven)
        {
            _interval = interval;
            _peer = peer;
            _entities = entities;
        }
        #endregion

        #region Lifecycle Entities
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _dirty = new Queue<NetworkEntity>();
            _newUsers = new Queue<User>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _entities.Events.TryAdd<Entity>("added", this.HandleEntityAdded);
            _entities.Events.TryAdd<Entity>("removed", this.HandleEntityRemoved);

            this.driven.Group.Users.Events.TryAdd<User>("added", this.HandleUserAdded);
        }

        protected override void Dispose()
        {
            base.Dispose();

            _dirty.Clear();
            _newUsers.Clear();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (_newUsers.Any())
                this.SetupNewUser(_newUsers.Dequeue());

            while (_dirty.Any())
                this.CreateUpdateMessage(_dirty.Dequeue());

            if (_interval.Is(100))
                _peer.FlushSendQueue();
        }
        #endregion

        #region Helper Methods
        private void SetupNewUser(User user)
        {
            this.driven.Group.CreateMessage("setup:start", user, NetDeliveryMethod.ReliableOrdered);

            _entities.ForEach(e =>
            { // Send all existing network entities to the new user
                if (e is NetworkEntity)
                    this.CreateCreateMessage(e as NetworkEntity, user);
            });

            this.driven.Group.CreateMessage("setup:end", user, NetDeliveryMethod.ReliableOrdered);
        }
        #endregion

        #region Message Methods
        private void CreateCreateMessage(NetworkEntity entity, User recipient = null)
        {
            var message = this.driven.Group.CreateMessage("entity:create", recipient, NetDeliveryMethod.ReliableOrdered);
            message.Write(entity.Configuration.Handle);
            message.Write(entity.Id);
            entity.TryWritePreInitialize(message);
            entity.TryWritePostInitialize(message);

            if (recipient == null)
                _dirty.Enqueue(entity);
            else
                this.CreateUpdateMessage(entity, recipient);
        }

        private void CreateUpdateMessage(NetworkEntity entity, User recipient = null)
        {
            var message = this.driven.Group.CreateMessage("entity:update", recipient, NetDeliveryMethod.ReliableOrdered);
            entity.TryWrite(message);
        }
        #endregion

        #region Event Handlers
        private void HandleEntityAdded(object sender, Entity entity)
        {
            if (entity is NetworkEntity) // Enqueue the new entity so the create message can be sent next frame
                this.CreateCreateMessage(entity as NetworkEntity);
        }

        private void HandleEntityRemoved(object sender, Entity entity)
        {
            if(entity is NetworkEntity)
            { // Broadcast the entity removal  to all clients
                var message = this.driven.Group.CreateMessage("entity:remove", NetDeliveryMethod.ReliableUnordered);
                message.Write(entity.Id);
            }
        }

        private void HandleUserAdded(object sender, User user)
        {
            _newUsers.Enqueue(user);
        }
        #endregion
    }
}
