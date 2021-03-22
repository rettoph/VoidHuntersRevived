using Guppy;
using Guppy.DependencyInjection;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Services.SpellCasts;
using VoidHuntersRevived.Library.Services.Spells;

namespace VoidHuntersRevived.Library.Services
{
    /// <summary>
    /// Simple service used for the creation &
    /// casting of spells. Note, the arguments
    /// are generic and must be known when
    /// casting.
    /// 
    /// (Extension methods may exist)
    /// </summary>
    public class SpellCastService : Service
    {
        #region Private Fields
        private ServiceProvider _provider;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _provider = provider;
        }

        protected override void Release()
        {
            base.Release();

            _provider = null;
        }
        #endregion

        #region API Methods
        public Spell TryCast(UInt32 spellCastId, SpellCaster caster, Single manaCost, Boolean force, params Object[] args)
            => _provider.GetService<SpellCast>(spellCastId).TryCast(caster, manaCost, force, args);
        #endregion
    }
}
