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
using VoidHuntersRevived.Common.Services;

namespace VoidHuntersRevived.Library
{
    public abstract class GameGuppy : FrameableGuppy
    {
        public readonly Peer Peer;
        public readonly NetScope NetScope;
        public readonly ISimulationService Simulations;

        protected GameGuppy(
            Peer peer, 
            NetScope netScope,
            ISimulationService simulations)
        {
            this.Peer = peer;
            this.NetScope = netScope;
            this.Simulations = simulations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Peer.Flush();

            this.Simulations.Update(gameTime);
        }
    }
}
