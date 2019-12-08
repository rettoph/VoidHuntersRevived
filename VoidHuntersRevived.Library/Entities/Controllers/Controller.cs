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
        #endregion

        #region Public Properties
        public IReadOnlyCollection<FarseerEntity> Components { get => _components; }
        public Boolean Dirty { get; protected set; }
        /// <summary>
        /// Indicates that the components withing this controller
        /// should be controlled soley by this controller.
        /// 
        /// That is, only the current controller should be
        /// manipulating the entity.
        /// 
        /// Used primarily to mark what controllers the TractorBeam
        /// is allowed to select from.
        /// </summary>
        public Boolean Locked { get; protected set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _components = new HashSet<FarseerEntity>();

            this.Locked = false;

            this.Events.Register<GameTime>("cleaned");
        }

        public override void Dispose()
        {
            base.Dispose();

            // Auto remove any remaining components
            while (_components.Any())
                this.Remove(_components.First());
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
                this.Events.TryInvoke<GameTime>(this, "cleaned", gameTime);

                this.Dirty = false;
            }
        }

        public virtual Boolean Add(FarseerEntity entity)
        {
            if(_components.Add(entity))
            {
                entity.SetController(this);
                this.Dirty = true;

                return true;
            }

            return false;
        }

        public virtual Boolean Remove(FarseerEntity entity)
        {
            if(_components.Remove(entity))
            {
                this.Dirty = true;

                return true;
            }

            return false;
        }
        #endregion
    }
}
