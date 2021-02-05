using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.System.Collections;
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
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Drivers.Scenes
{
    internal sealed class GameSceneMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<GameScene>
    {
        #region Private Fields
        private EntityList _entities;
        private ServiceList<NetworkEntity> _networkEntities;
        private Queue<NetworkEntity> _creates;
        private NetworkEntity _entity;
        private ServiceList<Player> _players;

        private Queue<User> _newUsers;
        private UserNetConnectionDictionary _userConnections;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeRemote(GameScene driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            _creates = new Queue<NetworkEntity>();
            _newUsers = new Queue<User>();

            provider.Service(out _entities);
            provider.Service(out _userConnections);
            provider.Service(out _networkEntities);
            provider.Service(out _players);

            this.driven.Group.Messages.Set(VHR.Pings.Scene.Entity, this.HandleEntityMessage);

            _entities.OnAdded += this.HandleEntityAdded;
            _networkEntities.OnRemoved += this.HandleNetworkEntityRemoved;
            this.driven.Group.Users.OnAdded += this.HandleUserJoined;
            this.driven.OnUpdate += this.Update;
            _players.OnAdded += this.HandlePlayerAdded;
        }

        protected override void ReleaseRemote(GameScene driven)
        {
            base.ReleaseRemote(driven);

            _entities.OnAdded -= this.HandleEntityAdded;
            _networkEntities.OnRemoved -= this.HandleNetworkEntityRemoved;
            this.driven.Group.Users.OnAdded -= this.HandleUserJoined;
            this.driven.OnUpdate -= this.Update;
            _players.OnAdded -= this.HandlePlayerAdded;

            _entities = null;
            _userConnections = null;
            _networkEntities = null;
            _players = null;
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

            this.driven.Group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1, connection).Write(VHR.Pings.Scene.Setup, om =>
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

            this.driven.Group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1, connection).Write(VHR.Pings.Scene.Setup, om =>
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
                case MessageType.Ping:
                    _entities.GetById<NetworkEntity>(im.ReadGuid()).MessageHandlers[MessageType.Ping].TryRead(im);
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

        private void HandlePlayerAdded(IServiceList<Player> sender, Player player)
        {
            if(player is ComputerPlayer ai)
            { // Bind to some player events...
                ai.Ship.OnBridgeChanged += this.HandleAIBridgeChanged;
            }
        }

        private void HandleAIBridgeChanged(Ship sender, ShipPart old, ShipPart value)
        {
            if(value == default)
            { // Destroy the ship & player.
                sender.OnBridgeChanged -= this.HandleAIBridgeChanged;

                sender.Player.TryRelease();
                sender.TryRelease();
            }
        }
        #endregion
    }
}
