using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.Lists.Interfaces;
using Guppy.Network;
using Guppy.Network.Interfaces;
using Guppy.Network.Security;
using Guppy.Network.Security.Enums;
using Guppy.Network.Security.EventArgs;
using Guppy.Network.Security.Lists;
using Guppy.Threading.Interfaces;
using Guppy.Threading.Utilities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using VoidHuntersRevived.Library.Entities.Aether;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Server.Messages;

namespace VoidHuntersRevived.Server.Scenes
{
    public class ServerPrimaryScene : PrimaryScene,
        IDataProcessor<UserJoinedMessage>
    {
        #region Private Fields
        private ChainService _chains;
        private PlayerService _players;
        private ShipService _ships;
        private ShipPartService _shipParts;
        private MessageBus _bus;
        private ChunkManager _chunks;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _players);
            provider.Service(out _ships);
            provider.Service(out _chains);
            provider.Service(out _shipParts);
            provider.Service(out _bus);
            provider.Service(out _chunks);

            _bus.GetQueue(Int32.MinValue).RegisterType<UserJoinedMessage>();
            _bus.RegisterProcessor<UserJoinedMessage>(this);

            this.Room.Users.OnEvent += this.HandleUserListEvent;
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.Room.Users.OnEvent -= this.HandleUserListEvent;
        }
        #endregion

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #region Event Handlers
        private void HandleUserListEvent(UserList sender, UserListEventArgs args)
        {
            switch(args.Action)
            {
                case UserListAction.Added:
                    this.HandleUserAdded(args.User);
                    break;
            }
        }
        private void HandleUserAdded(User user)
        {
            _bus.Enqueue(new UserJoinedMessage()
            {
                User = user
            });
        }

        bool IDataProcessor<UserJoinedMessage>.Process(UserJoinedMessage data)
        {
            this.Room.Pipes.GetById(PipeIds.PlayersPipeId).Users.TryAdd(data.User);

            if (data.User.IsCurrentUser)
            {
                return true;
            }

            // This is where we should create a new ship & chain for the user...
            ShipPart oldPart, newPart;

            oldPart = _shipParts.Create("ship-part:hull:square");

            for (int i = 0; i < 2; i++)
            {
                newPart = _shipParts.Create(ShipParts.HullSquare);
                oldPart.ConnectionNodes[2].TryAttach(newPart.ConnectionNodes[0]);

                oldPart = newPart;
            }


            ConnectionNode[] nodes = oldPart.Root
                .GetChildren()
                .SelectMany(sp => sp.ConnectionNodes)
                .Where(cn => cn.Connection.State == ConnectionNodeState.Estranged)
                .ToArray();

            foreach(ConnectionNode node in nodes)
            {
                var thruster = _shipParts.Create(ShipParts.Thruster);
                node.TryAttach(thruster.ConnectionNodes[0]);
            }

            // Create a new player instance linked to the user.
            var chain = _chains.Create(
                shipPart: oldPart.Root,
                position: Random.Shared.NextVector2(-10, 10));

            // chain.Body.ApplyLinearImpulse(new Vector2(100, 0));

            var ship = _ships.Create(chain, _players.CreateUserPlayer(data.User));

            return true;
        }
        #endregion
    }
}
