using Guppy;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;
using System.Linq;
using VoidHuntersRevived.Library.Extensions.System.Collections;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Library.Entities.Controllers
{
    /// <summary>
    /// Controllers are entities that will 
    /// update self contained ShipParts as
    /// needed.
    /// </summary>
    public class Controller : Entity
    {
        #region Private Fields
        private HashSet<ShipPart> _parts;
        private GameAuthorization _authorization;
        private Queue<ShipPart> _synchronization;
        #endregion

        #region Protected Attributes
        protected IEnumerable<ShipPart> parts => _parts;
        #endregion

        #region Public Attributes
        /// <summary>
        /// Used to determin how a ShipPart should behave when
        /// container within the current controller.
        /// </summary>
        public GameAuthorization Authorization
        {
            get => _authorization;
            protected set
            {
                if(_authorization != value)
                {
                    if (this.OnAuthorizationChanged == null)
                        _authorization = value;
                    else
                        this.OnAuthorizationChanged.Invoke(this, _authorization, _authorization = value);
                }
            }
        }
        #endregion

        #region Events
        public GuppyDeltaEventHandler<Controller, GameAuthorization> OnAuthorizationChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _parts = new HashSet<ShipPart>();
            _synchronization = new Queue<ShipPart>();

            this.Authorization = provider.GetService<Settings>().Get<GameAuthorization>();
        }

        protected override void Release()
        {
            base.Release();

            _parts.Clear();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.FlushAll();
        }
        #endregion

        #region Helper Methods
        protected virtual void FlushAll()
        {
            while (_synchronization.Any())
                this.TryFlush(_synchronization.Dequeue());
        }

        private void TryFlush(ShipPart shipPart)
        {
            if (this.CanAdd(shipPart))
                this.Flush(shipPart);
        }

        protected virtual void Flush(ShipPart shipPart)
        {
            shipPart.Controller?.Remove(shipPart);
            _parts.Add(shipPart);
            shipPart.Controller = this;
        }

        protected virtual Boolean CanAdd(ShipPart shipPart)
            => shipPart != default(ShipPart) && shipPart.IsRoot;

        protected virtual void Add(ShipPart shipPart)
            => _synchronization.Enqueue(shipPart);

        protected virtual Boolean CanRemove(ShipPart shipPart)
            => _parts.Contains(shipPart) || _synchronization.Contains(shipPart);

        protected internal virtual void TryRemove(ShipPart shipPart)
        {
            if (this.CanRemove(shipPart))
                this.Remove(shipPart);
        }

        protected virtual void Remove(ShipPart shipPart)
        {
            if(_parts.Contains(shipPart))
                _parts.Remove(shipPart);

            if (_synchronization.Contains(shipPart))
                _synchronization.Remove(shipPart);

            if(shipPart.IsRoot) // Reset the controller if the part is still root.
                shipPart.Controller = null;
        }
        #endregion
    }
}
