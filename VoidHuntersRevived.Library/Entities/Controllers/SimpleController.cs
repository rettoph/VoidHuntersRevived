using Guppy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.Controllers
{
    public class SimpleController : Controller
    {
        #region Private Fields
        private Annex _annex;
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _annex = provider.GetRequiredService<Annex>();
        }
        #endregion

        #region Helper Methods
        public override Boolean Add(FarseerEntity entity)
        {
            if (base.Add(entity))
            {
                entity.Events.TryAdd<Creatable>("disposing", this.HandleComponentDisposing);

                return true;
            }

            return false;
        }

        public override Boolean Remove(FarseerEntity entity)
        {
            if (base.Remove(entity))
            {
                entity.Events.TryRemove<Creatable>("disposing", this.HandleComponentDisposing);

                return true;
            }

            return false;
        }
        #endregion

        #region Event Handlers
        protected virtual void HandleComponentDisposing(object sender, Creatable arg)
        {
            var entity = arg as FarseerEntity;
            // Auto remove the item from the current controller
            this.Remove(entity);
            entity.SetController(_annex);
        }
        #endregion
    }
}
