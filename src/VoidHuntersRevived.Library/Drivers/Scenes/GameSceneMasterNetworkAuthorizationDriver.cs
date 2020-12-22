using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.System;
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
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Drivers.Scenes
{
    internal sealed class GameSceneMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<GameScene>
    {
        #region Private Fields
        private EntityList _entities;
        private ServiceList<NetworkEntity> _networkEntities;
        private Queue<NetworkEntity> _creates;
        private NetworkEntity _entity;

        private Queue<User> _newUsers;
        private UserNetConnectionDictionary _userConnections;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GameScene driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            _creates = new Queue<NetworkEntity>();
            _newUsers = new Queue<User>();

            provider.Service(out _entities);
            provider.Service(out _userConnections);
            provider.Service(out _networkEntities);

            this.driven.Group.Messages.Set("entity:message", this.HandleEntityMessage);

            _entities.OnAdded += this.HandleEntityAdded;
            _networkEntities.OnRemoved += this.HandleNetworkEntityRemoved;
            this.driven.Group.Users.OnAdded += this.HandleUserJoined;
            this.driven.OnUpdate += this.Update;
        }

        protected override void Release(GameScene driven)
        {
            base.Release(driven);

            _entities.OnAdded -= this.HandleEntityAdded;
            _networkEntities.OnRemoved -= this.HandleNetworkEntityRemoved;
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
                _networkEntities.TryAdd(_entity);

                this.driven.Group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1).Then(create =>
                { // Broadcast a create message...
                    _entity.MessageHandlers[MessageType.Create].TryWrite(create);

                    this.driven.Group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1).Then(setup =>
                    { // Broadcast a setup message...
                        _entity.MessageHandlers[MessageType.Setup].TryWrite(setup);
                    });
                });
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

            _networkEntities.ForEach(entity =>
            {
                this.driven.Group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1, connection).Then(create =>
                { // Broadcast a create message...
                    entity.MessageHandlers[MessageType.Create].TryWrite(create);

                    this.driven.Group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1, connection).Then(setup =>
                    { // Broadcast a setup message...
                        entity.MessageHandlers[MessageType.Setup].TryWrite(setup);
                    });
                });
            });

            this.driven.Group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1, connection).Write("scene:setup", om =>
            { // Send true bit, representing complete setup...
                om.Write(true);
            });
        }
        #endregion

        #region Message Handlers
        private Boolean HandleEntityMessage(NetIncomingMessage im)
        {
            switch((MessageType)im.ReadByte())
            {
                case MessageType.Action:
                    _entities.GetById<NetworkEntity>(im.ReadGuid()).MessageHandlers[MessageType.Action].TryRead(im);
                    break;
                default:
                    throw new Exception("Invalid message type recieved.");
            }

            im.Recycle();

            return false;
        }
        #endregion

        #region Event Handlers
        private void HandleEntityAdded(IServiceList<Entity> sender, Entity entity)
        {
            if (entity is NetworkEntity ne)
                _creates.Enqueue(ne);
        }

        private void HandleNetworkEntityRemoved(IServiceList<NetworkEntity> sender, NetworkEntity entity)
        {
            this.driven.Group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1).Then(om =>
            { // Broadcast the removal data through the network.
                entity.MessageHandlers[MessageType.Remove].TryWrite(om);
            });
        }

        private void HandleUserJoined(IServiceList<User> sender, User user)
            => _newUsers.Enqueue(user);
        #endregion
    }
}
