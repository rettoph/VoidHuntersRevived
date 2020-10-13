using Guppy;
using Guppy.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Utilities;
using Guppy.Network.Extensions.Lidgren;
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
using Guppy.Extensions.System;
using VoidHuntersRevived.Library.Drivers;
using Guppy.Lists;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Drivers.Scenes;
using System.Collections.Concurrent;
using Guppy.Extensions.DependencyInjection;
using System.IO;
using VoidHuntersRevived.Library.Extensions.System;
using VoidHuntersRevived.Library.Entities.Controllers;

namespace VoidHuntersRevived.Drivers.Scenes
{
    /// <summary>
    /// Primary driver that will manage broadcasting create messages
    /// to all connected peers (assuming that the current peer has
    /// full authority)
    /// 
    /// If full authority is not currently granted, this will
    /// do nothing.
    /// </summary>
    internal sealed class GameSceneFullAuthorityNetworkDriver : GameSceneNetworkDriver
    {
        #region Private Fields
        private EntityList _entities;
        private List<NetworkEntity> _networkEntities;
        private ConcurrentQueue<NetworkEntity> _created;
        private ConcurrentQueue<NetworkEntity> _setups;
        private ConcurrentQueue<NetworkEntity> _removed;
        private UserNetConnectionDictionary _userConnections;

        private ConcurrentQueue<User> _newUsers;
        private User _user;
        private NetworkEntity _entity;
        #endregion

        #region Lifecycle Methods
        protected override void ConfigureFull(ServiceProvider provider)
        {
            base.ConfigureFull(provider);

            this.ConfigureBase(provider);

            _networkEntities = new List<NetworkEntity>();
            _created = new ConcurrentQueue<NetworkEntity>();
            _setups = new ConcurrentQueue<NetworkEntity>();
            _removed = new ConcurrentQueue<NetworkEntity>();
            _newUsers = new ConcurrentQueue<User>();

            provider.Service(out _entities);
            provider.Service(out _userConnections);

            this.driven.OnUpdate += this.Update;
            this.driven.Entities.OnAdded += this.HandleEntityAdded;
            this.driven.Entities.OnRemoved += this.HandleEntityRemoved;
            this.driven.Group.Users.OnAdded += this.HandleUserJoined;
        }

        protected override void ReleaseFull()
        {
            base.Dispose();

            this.DisposeBase();

            this.driven.OnUpdate -= this.Update;
            this.driven.Entities.OnAdded -= this.HandleEntityAdded;
            this.driven.Entities.OnRemoved -= this.HandleEntityRemoved;
            this.driven.Group.Users.OnAdded -= this.HandleUserJoined;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (_newUsers.Any())
                if (_newUsers.TryDequeue(out _user))
                    this.SetupNewUser(_user);

            while (_created.Any())
            {
                Console.WriteLine(_created.First());

                _created.TryDequeue(out _entity);
                NetworkEntityMessageBuilder.BuildCreateMessage(this.driven.Group, _entity);
                _setups.Enqueue(_entity);
            }

            while (_setups.Any()) 
            {
                _setups.TryDequeue(out _entity);
                NetworkEntityMessageBuilder.BuildSetupMessage(this.driven.Group, _entity);
            }

            while (_removed.Any())
            {
                _removed.TryDequeue(out _entity);
                NetworkEntityMessageBuilder.BuildRemoveMessage(this.driven.Group, _entity);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Handle a newly joined user by setting up their user player
        /// and broadcasting the world data over to them
        /// </summary>
        /// <param name="user"></param>
        private void SetupNewUser(User user)
        {
            this.driven.Group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1, _userConnections.Connections[user]).Write("scene:setup", om =>
            { // Send false bit, representing incomplete setup...
                om.Write(false);
            });

            _networkEntities.ForEach(e => NetworkEntityMessageBuilder.BuildCreateMessage(this.driven.Group, e, _userConnections.Connections[user]));
            _networkEntities.ForEach(e => NetworkEntityMessageBuilder.BuildSetupMessage(this.driven.Group, e, _userConnections.Connections[user]));

            this.driven.Group.Messages.Create(NetDeliveryMethod.ReliableOrdered, 1, _userConnections.Connections[user]).Write("scene:setup", om =>
            { // Send true bit, representing complete setup...
                om.Write(true);
            });

            // Create a new ship instance for the player
            // Create a new UserPlayer instance for this user...
            _entities.Create<UserPlayer>((up, p, c) =>
            {
                up.User = user;
                up.Ship = _entities.Create<Ship>((s, p2, c2) =>
                {
                    s.Import(File.OpenRead("Ships/mosquito.vh"));
                    // s.SetBridge(_entities.Create<ShipPart>("entity:ship-part:chassis:mosquito"));
                    s.Bridge.Position = (new Random()).NextVector2(0, Chunk.Size * 5);
                });
            });
        }
        #endregion

        #region Event Handlers
        private void HandleEntityAdded(IEnumerable<Entity> sender, Entity arg)
        {
            if (arg is NetworkEntity)
            {
                _networkEntities.Add(arg as NetworkEntity);
                _created.Enqueue(arg as NetworkEntity);
            }
        }

        private void HandleEntityRemoved(IEnumerable<Entity> sender, Entity arg)
        {
            if (arg is NetworkEntity)
            {
                _networkEntities.Remove(arg as NetworkEntity);
                _removed.Enqueue(arg as NetworkEntity);
            }
        }

        private void HandleUserJoined(IEnumerable<User> sender, User arg)
            => _newUsers.Enqueue(arg);
        #endregion
    }
}
