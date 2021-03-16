using Guppy.DependencyInjection;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Lidgren.Network;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Services.Spells.AmmunitionSpells;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipParts
{
    internal sealed class ShipPartMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<ShipPart>
    {
        #region Private Fields
        private ActionTimer _pingHealthTimer;
        private Synchronizer _synchronizer;
        private Boolean _dirtyHealth;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ShipPart driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            this.driven.OnApplyAmmunitionCollision += this.HandleApplyAmmunitionCollision;
        }

        protected override void Release(ShipPart driven)
        {
            this.driven.OnApplyAmmunitionCollision -= this.HandleApplyAmmunitionCollision;

            base.Release(driven);
        }


        protected override void InitializeRemote(ShipPart driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            _pingHealthTimer = new ActionTimer(150);
            _dirtyHealth = false;

            provider.Service(out _synchronizer);

            this.driven.MessageHandlers[MessageType.Setup].OnWrite += this.driven.WriteMaleConnectionNode;
            this.driven.MessageHandlers[MessageType.Setup].OnWrite += this.driven.WriteHealth;
            this.driven.MessageHandlers[MessageType.Update].OnWrite += this.driven.WriteHealth;

            this.driven.OnHealthChanged += this.HandleHealthChanged;
        }

        protected override void ReleaseRemote(ShipPart driven)
        {
            base.ReleaseRemote(driven);

            _synchronizer = null;
            _pingHealthTimer = null;

            this.driven.MessageHandlers[MessageType.Setup].OnWrite -= this.driven.WriteMaleConnectionNode;
            this.driven.MessageHandlers[MessageType.Setup].OnWrite -= this.driven.WriteHealth;
            this.driven.MessageHandlers[MessageType.Update].OnWrite -= this.driven.WriteHealth;

            this.driven.OnHealthChanged -= this.HandleHealthChanged;
        }
        #endregion

        #region Event Handlers
        private void HandleHealthChanged(ShipPart sender, float old, float value)
        {
            if (this.driven.IsRoot && !_dirtyHealth)
                return;

            _dirtyHealth = true;

            _synchronizer.Enqueue(gt =>
            {
                _pingHealthTimer?.Update(gt, gt =>
                {
                    // Broadcast a health update ONLY if the ship part is not a root piece.
                    // This is done in an effort to minimize the message count, as the root piece
                    // Already recieves packet updates.
                    this.driven.Ping.Create(VHR.Network.MessageData.ShipPart.UpdateHealthPing.NetDeliveryMethod, VHR.Network.MessageData.ShipPart.UpdateHealthPing.SequenceChannel).Write(
                        VHR.Network.Pings.ShipPart.UpdateHealth,
                        this.driven.WriteHealth);

                    _dirtyHealth = false;
                });
            });
        }

        private void HandleApplyAmmunitionCollision(IAmmunitionSpellTarget sender, AmmunitionSpell.CollisionData data, GameTime gameTime)
        {
            if(sender.ValidateAmmunitionCollision(data))
                this.driven.TryApplyDamage(
                    data.Ammunition.GetDamage(
                        data, 
                        gameTime));
        }
        #endregion
    }
}
