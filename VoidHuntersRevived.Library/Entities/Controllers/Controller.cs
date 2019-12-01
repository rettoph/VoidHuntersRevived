using FarseerPhysics.Dynamics;
using Guppy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.Controllers
{
    /// <summary>
    /// All farseer entities must reside within a controller.
    /// These will update, draw, and manage each self contained
    /// entity. Example's of uses for controllers would be Chunks,
    /// A Ship, and TracorBeams
    /// </summary>
    public abstract class Controller : Entity
    {
        #region Private Fields
        private HashSet<FarseerEntity> _components;
        private Queue<FarseerEntity> _added;
        #endregion

        #region Public Properties
        public IReadOnlyCollection<FarseerEntity> Components { get => _components; }
        public Boolean Dirty { get; protected set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _components = new HashSet<FarseerEntity>();
            _added = new Queue<FarseerEntity>();

            this.Events.Register<GameTime>("cleaned");
        }

        public override void Dispose()
        {
            base.Dispose();

            // Auto remove any remaining components
            while (_components.Any())
                this.Remove(_components.First());

            _added.Clear();
        }
        #endregion

        #region Setup Methods
        /// <summary>
        /// Setsup the recieved body, assuming that
        /// it is the internal farseer body linked to
        /// the given farseer entity.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="body"></param>
        public virtual void SetupBody(FarseerEntity component, Body body)
        {
            //
        }
        #endregion

        #region Frame Methods
        /// <summary>
        /// Updates the recieved body, assuming that
        /// it is the internal farseer body linked to
        /// the given farseer entity.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="body"></param>
        public virtual void UpdateBody(FarseerEntity component, Body body)
        {
            //
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.TryClean(gameTime);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Clean the current chunk
        /// </summary>
        private void TryClean(GameTime gameTime)
        {
            if (this.Dirty)
            { // Clean the chunk if dirty
                while (_added.Any()) // Auto update new components once
                    _added.Dequeue().TryUpdate(gameTime);

                this.Events.TryInvoke<GameTime>(this, "cleaned", gameTime);

                this.Dirty = false;
            }
        }

        public virtual Boolean Add(FarseerEntity entity)
        {
            if(_components.Add(entity))
            {
                _added.Enqueue(entity);
                entity.SetController(this);
                entity.Events.TryAdd<Creatable>("disposing", this.HandleComponentDisposing);
                this.Dirty = true;

                return true;
            }

            return false;
        }

        public virtual Boolean Remove(FarseerEntity entity)
        {
            if(_components.Remove(entity))
            {
                entity.Events.TryRemove<Creatable>("disposing", this.HandleComponentDisposing);
                this.Dirty = true;

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
            entity.SetController(null);
        }
        #endregion
    }
}
