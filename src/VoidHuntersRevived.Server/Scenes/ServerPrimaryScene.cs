using Guppy.DependencyInjection;
using Guppy.Lists.Interfaces;
using Guppy.Network.Interfaces;
using Guppy.Network.Lists;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Server.Scenes
{
    public class ServerPrimaryScene : PrimaryScene
    {
        #region Private Fields
        private ServerPeer _server;
        private ChainService _chains;
        private PlayerService _players;
        private ShipService _ships;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _server);
            provider.Service(out _players);
            provider.Service(out _ships);
            provider.Service(out _chains);

            _server.Users.OnAdded += this.HandleUserConnected;

            this.Channel.Users.OnAdded += this.HandleUserJoined;

            // provider.GetService<ChunkManager>().OnChunkAdded += (_, chunk) =>
            // {
            //     var chain = _chains.Create(
            //         contextName: "ship-part:hull:square",
            //         position: chunk.Bounds.Center.ToVector2());
            // 
            //     ShipPart child;
            //     Boolean result;
            // 
            //     child = provider.GetService<ShipPartService>().Create("ship-part:hull:square");
            //     result = chain.Root.ConnectionNodes[0].TryAttach(child.ConnectionNodes[3]);
            // 
            //     child = provider.GetService<ShipPartService>().Create("ship-part:hull:square");
            //     result = chain.Root.ConnectionNodes[1].TryAttach(child.ConnectionNodes[2]);
            // 
            //     child = provider.GetService<ShipPartService>().Create("ship-part:hull:square");
            //     result = chain.Root.ConnectionNodes[2].TryAttach(child.ConnectionNodes[1]);
            // 
            //     child = provider.GetService<ShipPartService>().Create("ship-part:hull:square");
            //     result = chain.Root.ConnectionNodes[3].TryAttach(child.ConnectionNodes[0]);
            // };
        }

        protected override void Release()
        {
            base.Release();

            this.Channel.Users.OnAdded -= this.HandleUserJoined;

            _server.Users.OnAdded -= this.HandleUserConnected;
            _server = default;
        }
        #endregion

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #region Event Handlers
        private void HandleUserJoined(IServiceList<IUser> sender, IUser user)
        {
            this.Channel.Pipes.GetOrCreateById(VoidHuntersRevived.Library.Constants.PipeIds.PlayersPipeId).Users.TryAdd(user);

            // Create a new player instance linked to the user.
            var chain = _chains.Create(
                contextName: "ship-part:hull:square",
                position: Vector2.Zero);

            _ships.Create(chain, _players.CreateUserPlayer(user));
        }

        private void HandleUserConnected(IServiceList<IUser> sender, IUser args)
        {
            this.Channel.Users.TryAdd(args);
        }
        #endregion
    }
}
