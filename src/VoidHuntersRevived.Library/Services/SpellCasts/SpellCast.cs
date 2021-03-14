using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Services.Spells;

namespace VoidHuntersRevived.Library.Services.SpellCasts
{
    /// <summary>
    /// A spellcast effectively acts as a factory for
    /// a creating a <see cref="Spell"/>.
    /// This will utilize a <see cref="SpellCaster"/> and 
    /// consume the required <see cref="SpellCaster.Mana"/>.
    /// </summary>
    public abstract class SpellCast : Service
    {
        #region Protected Properties
        protected OrderableList<Spell> spells { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.spells = provider.GetService<OrderableList<Spell>>();
        }

        protected override void Release()
        {
            base.Release();

            this.spells = null;
        }
        #endregion

        #region API Methods
        public virtual Spell TryCast(SpellCaster caster, Single manaCost, params Object[] args)
        {
            if(this.CanCast(caster, manaCost, args))
                return this.Cast(caster, manaCost, args);

            // No spell to be returned.
            return null;
        }

        /// <summary>
        /// Create & return a spell.
        /// </summary>
        /// <param name="caster">The caster responsible for casting this spell.</param>
        /// <param name="manaCost">The base mana cost for the current spell.</param>
        /// <param name="args">Generic & unknown arguments used internally by the spellcast.</param>
        /// <returns></returns>
        protected abstract Spell Cast(SpellCaster caster, Single manaCost, params Object[] args);

        /// <summary>
        /// Determin whether or nto a spell may be casted. (If not, return null)
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="manaCost"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual Boolean CanCast(SpellCaster caster, Single manaCost, params Object[] args)
            => caster?.TryConsumeMana(manaCost) ?? false;
        #endregion
    }
}
