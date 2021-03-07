using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Special
{
    /// <summary>
    /// Simple rigid part extension used for activatable
    /// components with spells.
    /// </summary>
    public abstract class SpellPart<TSender, TArgs> : RigidShipPart
        where TSender : RigidShipPart
    {
        #region Protected Properties
        protected SpellService spells { get; private set; }
        #endregion

        #region Events
        public ValidateEventDelegate<TSender, TArgs> OnValidateCast;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnValidateCast += SpellPart<TSender, TArgs>.HandleValidateCast;
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.spells = provider.GetService<SpellService>();
        }

        protected override void Release()
        {
            base.Release();

            this.spells = null;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnValidateCast -= SpellPart<TSender, TArgs>.HandleValidateCast;
        }
        #endregion

        #region Helper Methods
        public void TryCast(TArgs args)
        {
            var sender = this.GetSender();

            if (this.OnValidateCast.Validate(sender, args, true))
                this.Cast(args);
        }

        protected virtual TSender GetSender()
            => this as TSender;

        protected abstract void Cast(TArgs args);
        #endregion

        #region Event Handlers
        private static bool HandleValidateCast(TSender sender, TArgs args)
            => sender.Health > 0;
        #endregion
    }
}