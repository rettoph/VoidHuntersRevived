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
        #region Private Fields
        private Single _manaTarget;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeRemote(SpellCaster driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.MessageHandlers[MessageType.Update].OnRead += this.ReadUpdate;

            this.driven.OnPostUpdate += this.PostUpdate;
        }

        protected override void ReleaseRemote(SpellCaster driven)
        {
            base.ReleaseRemote(driven);

            this.driven.MessageHandlers[MessageType.Update].OnRead -= this.ReadUpdate;

            this.driven.OnPostUpdate -= this.PostUpdate;
        }
        #endregion

        #region Frame Methods
        private void PostUpdate(GameTime gameTime)
        {
            this.driven.Mana = _manaTarget;

            this.driven.Mana = Math.Min(
                this.driven.MaxMana, 
                Math.Max(
                    1, 
                    MathHelper.Lerp(
                        this.driven.Mana, 
                        _manaTarget, 
                        VHR.Utilities.SlaveLerpPerSecond * (Single)gameTime.ElapsedGameTime.TotalSeconds)));
        }
        #endregion

        #region Network Methods
        private void ReadUpdate(NetIncomingMessage im)
        {
            _manaTarget = im.ReadSingle();
            this.driven.Charging = im.ReadBoolean();
        }
        #endregion
    }
}
