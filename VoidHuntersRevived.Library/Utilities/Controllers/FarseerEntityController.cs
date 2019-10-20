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
    public abstract class FarseerEntityController<TControlled> : Driven, IController
        where TControlled : FarseerEntity
    {
        #region Private Fields
        private HashSet<TControlled> _components;
        #endregion

        #region Public Attributes
        public Color Color { get; set; }
        public Category CollidesWith { get; set; } = Categories.ActiveCollidesWith;
        public Category CollisionCategories { get; set; } = Categories.ActiveCollisionCategories;
        public Category IgnoreCCDWith { get; set; } = Categories.ActiveIgnoreCCDWith;
        public IReadOnlyCollection<TControlled> Components { get => _components; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _components = new HashSet<TControlled>();
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _components.ForEach(e => e.TryDraw(gameTime));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _components.ForEach(e => e.TryUpdate(gameTime));
        }
        #endregion

        #region Helper Methods
        public virtual Boolean Remove(TControlled entity)
        {
            if (entity.Controller == this && _components.Remove(entity))
            {
                entity.SetController(null);

                return true;
            }

            return false;
        }

        public virtual Boolean Add(TControlled entity)
        {
            if (entity.Controller != this)
            {
                // Auto remove the entity from its old controller before adding it to this one
                if (entity.Controller != default(IController))
                    entity.Controller.Remove(entity);

                if (_components.Add(entity))
                {
                    entity.SetController(this);

                    return true;
                }
            }

            return false;
        }

        public bool Add(Object entity)
        {
            if (entity is TControlled)
                return this.Add(entity as TControlled);

            return false;
        }

        public bool Remove(Object entity)
        {
            if (entity is TControlled)
                return this.Remove(entity as TControlled);

            return false;
        }
        #endregion
    }
}
