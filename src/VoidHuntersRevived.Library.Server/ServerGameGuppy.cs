using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Providers;
using Guppy.Network.Peers;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Library.Server
{
    public sealed class ServerGameGuppy : GameGuppy
    {
        public readonly ServerPeer Server;

        public ServerGameGuppy(ServerPeer server, NetScope netScope, ISimulationService simulations) : base(server, netScope, simulations)
        {
            this.Server = server;

            this.Server.Users.OnUserConnected += this.HandleUserConnected;
        }

        public override void Initialize(IServiceProvider provider)
        {
            this.Peer.Bind(this.NetScope, 0);

            this.Server.Start(1337);

            this.Simulations.Initialize(SimulationType.Lockstep);

            base.Initialize(provider);
        }

        private void HandleUserConnected(IUserProvider sender, User args)
        {
            this.Server.Scopes[0].Users.Add(args);
        }
    }
}