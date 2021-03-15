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

            this.OnValidateCast += this.ValidateCast;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnValidateCast -= this.ValidateCast;
        }
        #endregion

        #region Helper Methods
        public override Spell TryCast(GameTime gameTime, bool force = false)
        {
            if (this.Casting) // Auto stop the previous cast, if any.
                this.StopCast(force);

            return _spell = base.TryCast(gameTime, force);
        }

        /// <summary>
        /// Attempt to stop casting (if currently casting)
        /// the current spell.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        public virtual void StopCast(Boolean force = false)
        {
            _spell?.TryRelease();
            _spell = null;
        }
        #endregion

        #region Event Handlers
        private bool ValidateCast(SpellPart sender, GameTime args)
            => !this.Casting;
        #endregion
    }
}
