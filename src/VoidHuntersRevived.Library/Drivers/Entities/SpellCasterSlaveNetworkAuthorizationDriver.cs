using Guppy.DependencyInjection;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class SpellCasterSlaveNetworkAuthorizationDriver : SlaveNetworkAuthorizationDriver<SpellCaster>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(SpellCaster driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.MessageHandlers[MessageType.Update].OnRead += this.ReadUpdate;
        }

        protected override void ReleaseRemote(SpellCaster driven)
        {
            base.ReleaseRemote(driven);

            this.driven.MessageHandlers[MessageType.Update].OnRead -= this.ReadUpdate;
        }
        #endregion

        #region Network Methods
        private void ReadUpdate(NetIncomingMessage im)
        {
            this.driven.Mana = im.ReadSingle();
            this.driven.Charging = im.ReadBoolean();
        }
        #endregion
    }
}
