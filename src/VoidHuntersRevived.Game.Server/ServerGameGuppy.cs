using Autofac;
using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Providers;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Game.Server
{
    public sealed class ServerGameGuppy : GameGuppy
    {
        public readonly ServerPeer Server;
        public readonly NetScope NetScope;

        public ServerGameGuppy(ServerPeer server, NetScope netScope, ISimulationService simulations) : base(simulations)
        {
            this.Server = server;
            this.NetScope = netScope;

            this.Server.Users.OnUserConnected += this.HandleUserConnected;
        }

        public override void Initialize(ILifetimeScope scope)
        {
            this.Server.Bind(this.NetScope, NetScopeIds.Game);

            this.Server.Start(1337);


            this.Simulations.Configure(SimulationType.Lockstep);

            base.Initialize(scope);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Server.Flush();
        }

        private void HandleUserConnected(IUserProvider sender, User args)
        {
            this.Server.Scopes[NetScopeIds.Game].Users.Add(args);
        }
    }
}