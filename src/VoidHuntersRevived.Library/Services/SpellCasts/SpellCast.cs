using Guppy;
using Guppy.DependencyInjection;
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
        protected FrameableList<Spell> spells { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.spells = provider.GetService<FrameableList<Spell>>();
        }

        protected override void Release()
        {
            base.Release();

            this.spells = null;
        }
        #endregion

        #region API Methods
        public virtual Spell TryCast(SpellCaster caster, params Object[] args)
            => this.Cast(caster, args);

        /// <summary>
        /// Create & return a spell.
        /// </summary>
        /// <param name="caster">The caster responsible for casting this spell.</param>
        /// <param name="args">Generic & unknown arguments used internally by the spellcast.</param>
        /// <returns></returns>
        protected abstract Spell Cast(SpellCaster caster, params Object[] args);
        #endregion
    }
}
