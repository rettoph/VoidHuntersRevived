using Guppy;
using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Events.Delegates;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Services.Spells
{
    public abstract class Spell : Driven
    {
        #region Public Properties
        public SpellCaster Caster { get; internal set; }
        #endregion

        #region Events
        public event OnEventDelegate<Spell> OnInvoke;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnStatus[ServiceStatus.Ready] += this.HandleReady;
            this.OnInvoke += this.Invoke;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnStatus[ServiceStatus.Ready] -= this.HandleReady;
            this.OnInvoke -= this.Invoke;
        }
        #endregion

        #region Helper Methods
        protected virtual void Invoke(Spell spell)
        {
            // Simple method, called once when spell is ready.
        }
        #endregion

        #region Event Handlers
        private void HandleReady(IService sender)
            => this.OnInvoke?.Invoke(this);
        #endregion
    }
}
