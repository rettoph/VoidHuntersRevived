using Guppy.DependencyInjection;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipParts
{
    internal sealed class ShipPartMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<ShipPart>
    {
        #region Lifecycle Methods
        protected override void Initialize(ShipPart driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            this.driven.MessageHandlers[MessageType.Setup].OnWrite += this.driven.WriteMaleConnectionNode;
            this.driven.MessageHandlers[MessageType.Setup].OnWrite += this.driven.WriteHealth;
            this.driven.MessageHandlers[MessageType.Update].OnWrite += this.driven.WriteHealth;

            this.driven.OnHealthChanged += this.HandleHealthChanged;
        }

        protected override void Release(ShipPart driven)
        {
            base.Release(driven);

            this.driven.MessageHandlers[MessageType.Setup].OnWrite -= this.driven.WriteMaleConnectionNode;
            this.driven.MessageHandlers[MessageType.Setup].OnWrite -= this.driven.WriteHealth;
            this.driven.MessageHandlers[MessageType.Update].OnWrite -= this.driven.WriteHealth;

            this.driven.OnHealthChanged -= this.HandleHealthChanged;
        }
        #endregion

        #region Event Handlers
        private void HandleHealthChanged(ShipPart sender, float old, float value)
        {
            this.driven.Actions.Create(NetDeliveryMethod.Unreliable, 0).Write("update:health", om =>
            {
                this.driven.WriteHealth(om);
            });
        }
        #endregion
    }
}
