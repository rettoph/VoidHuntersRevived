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
using System.Collections.Concurrent;

namespace VoidHuntersRevived.Server.Drivers
{
    [IsDriver(typeof(NetworkScene))]
    internal class ServerNetworkSceneDriver : Driver<NetworkScene>
    {
        #region Private Fields
        private EntityCollection _entities;
        private ConcurrentQueue<User> _newUsers;
        private ConcurrentQueue<NetworkEntity> _updates;
        private ConcurrentQueue<NetworkEntity> _creates;
        private ConcurrentQueue<Guid> _removes;
        private Interval _interval;
        private NetPeer _peer;

        private User _user;
        private NetworkEntity _entity;
        private Guid _id;
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

            _newUsers = new ConcurrentQueue<User>();
            _creates = new ConcurrentQueue<NetworkEntity>();
            _updates = new ConcurrentQueue<NetworkEntity>();
            _removes = new ConcurrentQueue<Guid>();
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

            _updates.Clear();
            _newUsers.Clear();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (_newUsers.Any())
                if(_newUsers.TryDequeue(out _user))
                    this.SetupNewUser(_user);

            while (_creates.Any())
                if (_creates.TryDequeue(out _entity))
                    this.CreateCreateMessage(_entity);

            while (_updates.Any())
                if(_updates.TryDequeue(out _entity))
                this.CreateUpdateMessage(_entity);

            while (_removes.Any())
                if (_removes.TryDequeue(out _id))
                    this.CreateRemoveMessage(_id);

            if (_interval.Is(100))
                _peer.FlushSendQueue();
        }
        #endregion

        #region Helper Methods
        private void SetupNewUser(User user)
        {
            this.driven.Group.Messages.Create("setup:start", NetDeliveryMethod.ReliableOrdered, 0, user);

            _entities.ForEach(e =>
            { // Send all existing network entities to the new user
                if (e is NetworkEntity)
                    this.CreateCreateMessage(e as NetworkEntity, user);
            });

            this.driven.Group.Messages.Create("setup:end", NetDeliveryMethod.ReliableOrdered, 0, user);
        }
        #endregion

        #region Message Methods
        private void CreateCreateMessage(NetworkEntity entity, User recipient = null)
        {
            var message = this.driven.Group.Messages.Create("entity:create", NetDeliveryMethod.ReliableOrdered, 0, recipient);
            message.Write(entity.Configuration.Handle);
            message.Write(entity.Id);
            entity.TryWritePreInitialize(message);
            entity.TryWritePostInitialize(message);

            if (recipient == null)
                _updates.Enqueue(entity);
            else
                this.CreateUpdateMessage(entity, recipient);
        }

        private void CreateUpdateMessage(NetworkEntity entity, User recipient = null)
        {
            var message = this.driven.Group.Messages.Create("entity:update", NetDeliveryMethod.ReliableOrdered, 0, recipient);
            entity.TryWrite(message);
        }

        private void CreateRemoveMessage(Guid id)
        {
            var message = this.driven.Group.Messages.Create("entity:remove", NetDeliveryMethod.ReliableOrdered, 0);
            message.Write(id);
        }
        #endregion

        #region Event Handlers
        private void HandleEntityAdded(object sender, Entity entity)
        {
            if (entity is NetworkEntity) // Enqueue the new entity so the create message can be sent next frame
                _creates.Enqueue(entity as NetworkEntity);
        }

        private void HandleEntityRemoved(object sender, Entity entity)
        {
            if (entity is NetworkEntity)
                _removes.Enqueue(entity.Id);
        }

        private void HandleUserAdded(object sender, User user)
        {
            _newUsers.Enqueue(user);
        }
        #endregion
    }
}
