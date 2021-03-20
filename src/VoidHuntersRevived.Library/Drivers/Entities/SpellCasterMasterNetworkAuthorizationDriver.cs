using Guppy.DependencyInjection;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class SpellCasterMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<SpellCaster>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(SpellCaster driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.OnManaChanged += this.HandleManaChanged;

            this.driven.MessageHandlers[MessageType.Update].OnWrite += this.WriteUpdate;
        }

        protected override void ReleaseRemote(SpellCaster driven)
        {
            base.ReleaseRemote(driven);

            this.driven.MessageHandlers[MessageType.Update].OnWrite -= this.WriteUpdate;
        }
        #endregion

        #region Network Methods
        private void WriteUpdate(NetOutgoingMessage om)
        {
            om.Write(this.driven.Mana);
            om.Write(this.driven.Charging);
        }
        #endregion

        #region Event Handlers
        private void HandleManaChanged(SpellCaster sender, float old, float value)
        {
            if (this.driven.Mana >= this.driven.MaxMana)
            {
                this.driven.Mana = this.driven.MaxMana;
                this.driven.Charging = false;
            }

            if (this.driven.Mana <= 0)
            {
                this.driven.Charging = true;
                this.driven.Mana = 0;
            }
        }
        #endregion
    }
}
