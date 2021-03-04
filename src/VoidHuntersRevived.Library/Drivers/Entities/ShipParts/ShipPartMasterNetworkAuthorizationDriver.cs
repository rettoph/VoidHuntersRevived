using Guppy.DependencyInjection;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ammunitions;
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

            this.driven.OnApplyAmmunitionCollision += this.HandleApplyAmmunitionCollision;
        }


        protected override void InitializeRemote(ShipPart driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.MessageHandlers[MessageType.Setup].OnWrite += this.driven.WriteMaleConnectionNode;
            this.driven.MessageHandlers[MessageType.Setup].OnWrite += this.driven.WriteHealth;
            this.driven.MessageHandlers[MessageType.Update].OnWrite += this.driven.WriteHealth;

            this.driven.OnHealthChanged += this.RemoteHandleHealthChanged;
        }

        protected override void Release(ShipPart driven)
        {
            this.driven.OnHealthChanged -= this.RemoteHandleHealthChanged;

            base.Release(driven);
        }

        protected override void ReleaseRemote(ShipPart driven)
        {
            base.ReleaseRemote(driven);

            this.driven.MessageHandlers[MessageType.Setup].OnWrite -= this.driven.WriteMaleConnectionNode;
            this.driven.MessageHandlers[MessageType.Setup].OnWrite -= this.driven.WriteHealth;
            this.driven.MessageHandlers[MessageType.Update].OnWrite -= this.driven.WriteHealth;

            this.driven.OnHealthChanged -= this.RemoteHandleHealthChanged;
        }
        #endregion

        #region Event Handlers
        private void HandleApplyAmmunitionCollision(ShipPart sender, Ammunition.CollisionData data, GameTime gameTime)
        {
            if(this.driven.ValidateAmmunitionCollisionDamage(data))
                this.driven.TryApplyDamage(
                    data.Ammunition.GetDamage(
                        data, 
                        gameTime));
        }


        private void RemoteHandleHealthChanged(ShipPart sender, float old, float value)
        {
            this.driven.Ping.Create(NetDeliveryMethod.Unreliable, 0).Write(VHR.Pings.ShipPart.UpdateHealth, om =>
            {
                this.driven.WriteHealth(om);
            });
        }
        #endregion
    }
}
