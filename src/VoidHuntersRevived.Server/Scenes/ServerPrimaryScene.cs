using Guppy.DependencyInjection;
using Guppy.Lists.Interfaces;
using Guppy.Network.Interfaces;
using Guppy.Network.Lists;
using Guppy.Network.Peers;
using Guppy.Threading.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Globals.Constants;
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
        private ShipPartService _shipParts;

        private ThreadQueue _updateThread;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _server);
            provider.Service(out _players);
            provider.Service(out _ships);
            provider.Service(out _chains);
            provider.Service(out _updateThread);
            provider.Service(out _shipParts);

            _server.Users.OnAdded += this.HandleUserConnected;

            this.Channel.Users.OnAdded += this.HandleUserJoined;

            var rand = new Random();

            provider.GetService<ChunkManager>().OnChunkAdded += (_, chunk) =>
            {
                for(var i=0; i<40; i++)
                {
                    var chain = _chains.Create(
                        contextName: "ship-part:hull:square",
                        position: chunk.Bounds.Center.ToVector2() + rand.NextVector2(-(Chunk.Size / 2), (Chunk.Size / 2)));
                }
            };
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
            this.Channel.Pipes.GetOrCreateById(PipeIds.PlayersPipeId).Users.TryAdd(user);

            _updateThread.Enqueue(_ =>
            {
                ShipPart oldPart, newPart;

                oldPart = _shipParts.Create("ship-part:hull:square");

                //for (int i = 0; i < 20; i++)
                //{
                //    newPart = _shipParts.Create("ship-part:hull:square");
                //    oldPart.ConnectionNodes[2].TryAttach(newPart.ConnectionNodes[0]);
                //
                //    oldPart = newPart;
                //}

                // Create a new player instance linked to the user.
                var chain = _chains.Create(
                    shipPart: oldPart.Root,
                    position: Vector2.Zero);

                var ship = _ships.Create(chain, _players.CreateUserPlayer(user));
            });
        }

        private void HandleUserConnected(IServiceList<IUser> sender, IUser args)
        {
            this.Channel.Users.TryAdd(args);
        }
        #endregion
    }
}
