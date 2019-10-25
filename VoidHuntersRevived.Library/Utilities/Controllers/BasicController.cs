using Guppy;
using Guppy.Extensions.Collection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Utilities.Controllers
{
    /// <summary>
    /// Managed controllers will automatically return their components
    /// into the approproate chunk on removal
    /// </summary>
    /// <typeparam name="TControlled"></typeparam>
    public class BasicController<TControlled> : Controller<TControlled>
        where TControlled : FarseerEntity
    {
        private ChunkCollection _chunks;
        private Annex _annex;

        public BasicController(Annex annex, ChunkCollection chunks)
        {
            _chunks = chunks;
            _annex = annex;
        }

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.CollidesWith = Categories.PassiveCollidesWith;
            this.CollisionCategories = Categories.PassiveCollisionCategories;
            this.IgnoreCCDWith = Categories.PassiveIgnoreCCDWith;
        }
        #endregion

        #region Helper Methods
        protected override Boolean Add(TControlled entity)
        {
            if(base.Add(entity))
            {
                entity.Events.TryAdd<Creatable>("disposing", this.HandleComponentDisposing);
                return true;
            }

            return false;
        }

        protected override bool Remove(TControlled entity)
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
            // Return the item to the annex when it is disposed
            _annex.TryAdd(arg as FarseerEntity);
        }
        #endregion
    }
}
