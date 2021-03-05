using Guppy.DependencyInjection;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class WorldEntitySlaveNetworkAuthorizationDriver : SlaveNetworkAuthorizationDriver<WorldEntity>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(WorldEntity driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.Ping.Set(VHR.Pings.World.UpdateSize, this.driven.ReadSize);
            this.driven.Ping.Set(VHR.Pings.World.CreateExplosion, this.HandleCreateExplosionMessage);
        }


        protected override void ReleaseRemote(WorldEntity driven)
        {
            base.ReleaseRemote(driven);

            this.driven.Ping.Remove(VHR.Pings.World.UpdateSize);
            this.driven.Ping.Remove(VHR.Pings.World.CreateExplosion);
        }
        #endregion

        #region Message Handlers
        private void HandleCreateExplosionMessage(NetIncomingMessage im)
        {
            this.driven.EnqeueExplosion(new WorldEntity.ExplosionData(
                position: im.ReadVector2(),
                color: im.ReadColor(),
                radius: im.ReadSingle(),
                force: im.ReadSingle(),
                damage: im.ReadSingle()));
        }
        #endregion
    }
}
