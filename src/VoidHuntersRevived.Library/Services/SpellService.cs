using Guppy;
using Guppy.DependencyInjection;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Interfaces.Spells;

namespace VoidHuntersRevived.Library.Services
{
    public class SpellService : Service
    {
        #region Private Fields
        private GuppyServiceProvider _provider;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            _provider = provider;
        }
        #endregion
    }
}
