using Guppy;
using Guppy.DependencyInjection;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Services
{
    /// <summary>
    /// A simple service responsible for creating
    /// new explosion instances correctly.
    /// </summary>
    public class ExplosionService : Service
    {
        #region Private Fields
        private EntityList _entities;
        #endregion

        #region LIfecycle Methods 
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _entities);
        }
        #endregion

        #region API Methods
        public Explosion Create(ExplosionContext context)
        {
            return _entities.Create<Explosion>((e, p, c) =>
            {
                e.Context = context;
            });
        }
        #endregion
    }
}
