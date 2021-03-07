using Guppy;
using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Events.Delegates;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.Spells
{
    /// <summary>
    /// The base spell class capable of preforming unique actions
    /// in the game.
    /// </summary>
    public abstract class Spell : Entity
    {
        #region Events
        public event OnEventDelegate<Spell> OnCast;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnStatus[ServiceStatus.Ready] += this.HandleReady;
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.LayerGroup = VHR.LayersContexts.Spell.Group.GetValue();
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnStatus[ServiceStatus.Ready] -= this.HandleReady;
        }
        #endregion

        #region Helper Methods
        protected virtual void Cast()
        {
            //
        }
        #endregion

        #region Event Handlers
        private void HandleReady(IService sender)
        {
            this.Cast();
            this.OnCast?.Invoke(this);
        }
        #endregion
    }
}
