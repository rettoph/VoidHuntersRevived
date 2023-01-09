using Guppy.MonoGame;
using Guppy.MonoGame.Services;
using Guppy.Network.Peers;
using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain
{
    public abstract class GameGuppy : FrameableGuppy
    {
        private World _world;

        public readonly Peer Peer;
        public readonly NetScope NetScope;
        public readonly ISimulationService Simulations;

        protected GameGuppy(
            Peer peer, 
            NetScope netScope,
            ISimulationService simulations)
        {
            _world = default!;

            this.Peer = peer;
            this.NetScope = netScope;
            this.Simulations = simulations;
        }

        public override void Initialize(IServiceProvider provider)
        {
            base.Initialize(provider);

            _world = provider.GetRequiredService<World>();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Peer.Flush();

            this.Simulations.Update(gameTime);

            _world.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _world.Draw(gameTime);
        }
    }
}
