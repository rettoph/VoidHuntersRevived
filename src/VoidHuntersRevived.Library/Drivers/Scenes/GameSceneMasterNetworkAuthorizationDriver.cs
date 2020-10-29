using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Lists;
using Guppy.Lists.Interfaces;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Drivers.Scenes
{
    internal sealed class GameSceneMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<GameScene>
    {
        #region Private Fields
        private EntityList _entities;
        private List<NetworkEntity> _networkEntities;
        private Queue<NetworkEntity> _creates;
        private Queue<NetworkEntity> _removes;
        private NetworkEntity _entity;

        private Queue<User> _newUsers;
        private UserNetConnectionDictionary _userConnections;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GameScene driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            _networkEntities = new List<NetworkEntity>();
            _creates = new Queue<NetworkEntity>();
            _removes = new Queue<NetworkEntity>();
            _newUsers = new Queue<User>();

            provider.Service(out _entities);
            provider.Service(out _userConnections);

            _entities.OnAdded += this.HandleEntityAdded;
            _entities.OnRemoved += this.HandleEntityRemoved;
            this.driven.Group.Users.OnAdded += this.HandleUserJoined;
            this.driven.OnUpdate += this.Update;
        }

        protected override void Release(GameScene driven)
        {
            base.Release(driven);

            _entities.OnAdded -= this.HandleEntityAdded;
            _entities.OnRemoved -= this.HandleEntityRemoved;
            this.driven.Group.Users.OnAdded -= this.HandleUserJoined;
            this.driven.OnUpdate -= this.Update;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            while (_newUsers.Any())
                this.SetupNewUser(_newUsers.Dequeue());

            while (_creates.Any())
            { // Broadcast all new network entities...
                _entity = _creates.Dequeue();
                _networkEntities.Add(_entity);
                NetworkEntityMessageBuilder.BuildCreateMessage(this.driven.Group, _entity);
            }

            while (_removes.Any())
            { // Broadcast all removed network entities...
                _entity = _removes.Dequeue();
                _networkEntities.Remove(_entity);
                NetworkEntityMessageBuilder.BuildRemoveMessage(this.driven.Group, _entity);
            }
        }
        #endregion

        #region Helper Methods
        private void SetupNewUser(User user)
        {
            var connection = _userConnections.Connections[user];

            this.driven.Group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1, connection).Write("scene:setup", om =>
            { // Send false bit, representing incomplete setup...
                om.Write(false);
            });

            _networkEntities.ForEach(e =>
            {
                NetworkEntityMessageBuilder.BuildCreateMessage(this.driven.Group, e, connection);
            });

            this.driven.Group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1, connection).Write("scene:setup", om =>
            { // Send true bit, representing complete setup...
                om.Write(true);
            });
        }
        #endregion

        #region Event Handlers
        private void HandleEntityAdded(IServiceList<Entity> sender, Entity entity)
        {
            if (entity is NetworkEntity)
                _creates.Enqueue(entity as NetworkEntity);
        }

        private void HandleEntityRemoved(IServiceList<Entity> sender, Entity entity)
        {
            if (entity is NetworkEntity)
                _removes.Enqueue(entity as NetworkEntity);
        }

        private void HandleUserJoined(IServiceList<User> sender, User user)
            => _newUsers.Enqueue(user);
        #endregion
    }
}
