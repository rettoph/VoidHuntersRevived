using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Special
{
    /// <summary>
    /// Simple rigid part extension used for activatable
    /// components with spells.
    /// </summary>
    public abstract class SpellPart : ShipPart
    {
        #region Private Fields
        private Single _lastCastTimestamp;
        #endregion

        #region Protected Properties
        protected SpellCastService spells { get; private set; }
        #endregion

        #region Public Properties
        public new SpellCasterPartContext Context { get; private set; }

        public Single LastCastTimestamp => _lastCastTimestamp;
        #endregion

        #region Events
        public event ValidateEventDelegate<SpellPart, GameTime> OnValidateCast;
        public event OnEventDelegate<SpellPart, GameTime> OnCast;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnValidateCast += SpellPart.HandleValidateCast;
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.spells = provider.GetService<SpellCastService>();
        }

        protected override void Release()
        {
            base.Release();

            this.spells = null;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnValidateCast -= SpellPart.HandleValidateCast;
        }
        #endregion

        #region Helper Methods
        public void TryCast(GameTime gameTime, Boolean force = false)
        {
            if (this.OnValidateCast.Validate(this, gameTime, true) || force)
            {
                _lastCastTimestamp = (Single)gameTime.TotalGameTime.TotalSeconds;
                this.Cast(gameTime);
                this.OnCast?.Invoke(this, gameTime);
            }
        }

        protected abstract void Cast(GameTime gameTime);

        public override void SetContext(ShipPartContext context)
        {
            base.SetContext(context);

            this.Context = context as SpellCasterPartContext;
        }
        #endregion

        #region Event Handlers
        private static bool HandleValidateCast(SpellPart sender, GameTime gameTime)
            => sender.Health > 0 
                && (gameTime.TotalGameTime.TotalSeconds - sender._lastCastTimestamp > sender.Context.SpellCooldown || sender._lastCastTimestamp == default)
                && (sender.Chain?.Ship?.CanConsumeMana(sender.Context.SpellManaCost) ?? false);
        #endregion
    }
}