using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Extensions.Collection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Utilities.Controllers
{
    /// <summary>
    /// Controllers represent objects that can own and manage
    /// ShipParts. They are responsible for updating and drawing
    /// all seld contained ShipParts.
    /// </summary>
    public abstract class Controller<TControlled> : Controller
        where TControlled : FarseerEntity
    {
        #region Protected Fields
        private HashSet<TControlled> _components;
        #endregion

        #region Public Attributes
        public IReadOnlyCollection<TControlled> Components { get => _components; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _components = new HashSet<TControlled>();

            this.Events.Register<TControlled>("added");
            this.Events.Register<TControlled>("removed");
        }
        #endregion

        #region Helper Methods
        protected virtual Boolean Remove(TControlled entity)
        {
            if(entity.Controller == this && _components.Remove(entity))
            {
                this.Events.TryInvoke<TControlled>(this, "removed", entity);

                return true;
            }

            return false;
        }

        public virtual Boolean Add(TControlled entity)
        {
            if (entity.Controller != this)
            {
                entity.Controller?.Remove(entity);

                if (_components.Add(entity))
                {
                    entity.SetController(this);

                    this.Events.TryInvoke<TControlled>(this, "added", entity);

                    return true;
                }
            }

            return false;
        }

        protected internal override void Remove(Object entity)
        {
            if (entity is TControlled)
                this.Remove(entity as TControlled);
        }
        #endregion
    }
}
