using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Services.Spells;

namespace VoidHuntersRevived.Library.Entities.ShipParts.SpellParts
{
    /// <summary>
    /// Simple implementation of <see cref="SpellPart"/>
    /// used to ease the creation of spells that are linked to
    /// a single part long term (such as shield generators)
    /// </summary>
    public abstract class ContinuousSpellPart : SpellPart
    {
        #region Private Fields
        private Spell _spell;
        #endregion

        #region Public Properties
        public Boolean Casting => _spell != default;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnRequiredValidateCast += this.HandleRequiredValidateCast;
        }



        protected override void Dispose()
        {
            base.Dispose();

            this.OnRequiredValidateCast -= this.HandleRequiredValidateCast;
        }
        #endregion

        #region Helper Methods
        /// <inheritdoc />
        public override Spell TryCast(GameTime gameTime, bool force = false)
        {
            _spell = base.TryCast(gameTime, force) ?? _spell;
            return _spell;
        }

        /// <summary>
        /// Attempt to stop casting (if currently casting)
        /// the current spell.
        /// </summary>
        /// <returns></returns>
        public virtual void StopCast()
        {
            _spell?.TryRelease();
            _spell = null;
        }
        #endregion

        #region Event Handlers
        private bool HandleRequiredValidateCast(SpellPart sender, GameTime args)
            => !this.Casting;
        #endregion
    }
}
