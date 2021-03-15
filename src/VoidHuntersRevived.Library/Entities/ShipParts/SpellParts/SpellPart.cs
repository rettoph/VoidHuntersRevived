using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Events.Delegates;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Services.Spells;

namespace VoidHuntersRevived.Library.Entities.ShipParts.SpellParts
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
        public new SpellPartContext Context { get; private set; }

        public Single LastCastTimestamp => _lastCastTimestamp;
        #endregion

        #region Events
        public event ValidateEventDelegate<SpellPart, GameTime> OnValidateCast;
        public event ValidateEventDelegate<SpellPart, GameTime> OnRequiredValidateCast;
        public event OnEventDelegate<SpellPart, GameTime> OnCast;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnValidateCast += SpellPart.HandleValidateCast;
            this.OnRequiredValidateCast += SpellPart.HandleRequiredValidateCast;
        }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _lastCastTimestamp = default;

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
            this.OnRequiredValidateCast -= SpellPart.HandleRequiredValidateCast;
        }
        #endregion

        #region Helper Methods
        public virtual Spell TryCast(GameTime gameTime, Boolean force = false)
        {
            if (this.OnRequiredValidateCast.Validate(this, gameTime, false))
            {
                if (this.OnValidateCast.Validate(this, gameTime, false) || (force && this.Status == ServiceStatus.Ready))
                {
                    _lastCastTimestamp = (Single)gameTime.TotalGameTime.TotalSeconds;
                    this.OnCast?.Invoke(this, gameTime);
                    return this.Cast(gameTime);
                }
            }

            return default;
        }

        protected abstract Spell Cast(GameTime gameTime);

        public override void SetContext(ShipPartContext context)
        {
            base.SetContext(context);

            this.Context = context as SpellPartContext;
        }
        #endregion

        #region Event Handlers
        private static bool HandleValidateCast(SpellPart sender, GameTime gameTime)
            => sender.Health > 0 
                && (gameTime.TotalGameTime.TotalSeconds - sender._lastCastTimestamp > sender.Context.SpellCooldown || sender._lastCastTimestamp == default)
                && sender.Chain.Ship.CanConsumeMana(sender.Context.SpellManaCost);

        private static bool HandleRequiredValidateCast(SpellPart sender, GameTime args)
            => sender.Chain?.Ship != default;
        #endregion
    }
}