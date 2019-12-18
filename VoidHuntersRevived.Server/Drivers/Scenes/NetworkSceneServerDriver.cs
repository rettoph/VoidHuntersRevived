using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Extensions.Collection;
using Guppy.Network.Security;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Scenes;
using Guppy.Network.Extensions.Lidgren;
using System.Linq;
using VoidHuntersRevived.Library.Extensions.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Server.Utilities;
using VoidHuntersRevived.Library.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Server.Drivers.Scenes
{
    /// <summary>
    /// Automatically send Create, Update, and Remove messages
    /// for new NetworkEntity instances.
    /// </summary>
    [IsDriver(typeof(NetworkScene))]
    public class NetworkSceneServerDriver : Driver<NetworkScene>
    {
        #region Static Properties
        private static Double VitalsPingRate { get; set; } = 75;
        #endregion

        #region Private Fields
        private NetPeer _peer;
        private EntityCollection _entities;
        private ConcurrentQueue<User> _newUsers;
        private ConcurrentQueue<NetworkEntity> _creates;
        private ConcurrentQueue<NetworkEntity> _updates;
        private ConcurrentQueue<Guid> _removes;

        private User _user;
        private NetworkEntity _entity;
        private Guid _id;

        private VitalsManager _vitals;
        private ActionTimer _vitalPingTimer;
        #endregion

        #region Constructor
        public NetworkSceneServerDriver(VitalsManager vitals, NetworkScene driven, NetPeer peer, EntityCollection entities) : base(driven)
        {
            _peer = peer;
            _entities = entities;
            _vitals = vitals;
        }
        #endregion

        #region Lifecycle Methods
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

            _vitalPingTimer = new ActionTimer(NetworkSceneServerDriver.VitalsPingRate);

            _entities.OnAdded += this.HandleEntityAdded;
            _entities.OnRemoved += this.HandleEntityRemoved;
            this.driven.Group.Users.OnAdded += this.HandleUserAdded;
        }

        protected override void Dispose()
        {
            base.Dispose();

            _entities.OnAdded -= this.HandleEntityAdded;
            _entities.OnRemoved -= this.HandleEntityRemoved;
            this.driven.Group.Users.OnAdded -= this.HandleUserAdded;

            _newUsers.Clear();
            _creates.Clear();
            _updates.Clear();
            _removes.Clear();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (_newUsers.Any())
                if (_newUsers.TryDequeue(out _user))
                    this.SetupNewUser(_user);

            while (_creates.Any())
                if (_creates.TryDequeue(out _entity))
                    this.CreateCreateMessage(_entity);

            while (_updates.Any())
                if (_updates.TryDequeue(out _entity))
                    this.CreateUpdateMessage(_entity);

            while (_removes.Any())
                if (_removes.TryDequeue(out _id))
                    this.CreateRemoveMessage(_id);

            // Automatically flush the vitals on the defined interval
            _vitalPingTimer.Update(
                gameTime: gameTime,
                action: _vitals.Flush);

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
            entity.TryWriteSetup(message);

            // Create an update message at the same time
            this.CreateUpdateMessage(entity, recipient);
        }

        private void CreateUpdateMessage(NetworkEntity entity, User recipient = null)
        {
            var message = this.driven.Group.Messages.Create("entity:update", NetDeliveryMethod.ReliableOrdered, 0, recipient);
            message.Write(entity);
            entity.TryWrite(message);
        }

        private void CreateRemoveMessage(Guid id)
        {
            var message = this.driven.Group.Messages.Create("entity:remove", NetDeliveryMethod.ReliableOrdered, 0);
            message.Write(id);
        }
        #endregion

        #region Event Handlers
        private void HandleEntityAdded(object sender, Entity arg)
        {
            if (arg is NetworkEntity)
            {
                _creates.Enqueue(arg as NetworkEntity);
                (arg as NetworkEntity).OnCleaned += this.HandleEntityCleaned;
            }
        }

        private void HandleEntityCleaned(object sender, GameTime arg)
        {
            _updates.Enqueue(sender as NetworkEntity);
        }

        private void HandleEntityRemoved(object sender, Entity arg)
        {
            if (arg is NetworkEntity)
            {
                _removes.Enqueue((arg as NetworkEntity).Id);
                (arg as NetworkEntity).OnCleaned -= this.HandleEntityCleaned;
            }
        }

        private void HandleUserAdded(object sender, User arg)
        {
            _newUsers.Enqueue(arg);
        }
        #endregion
    }
}
