using Guppy;
using Guppy.DependencyInjection;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Spells;

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
    public class SpellService : Service
    {
        #region Private Fields
        private EntityList _entities;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _entities);
        }

        protected override void Release()
        {
            base.Release();

            _entities = null;
        }
        #endregion

        #region API Methods
        public TSpell Cast<TSpell>(Action<TSpell, ServiceProvider, ServiceConfiguration> setup)
            where TSpell : Spell
                => _entities.Create<TSpell>(setup);
        #endregion
    }
}
