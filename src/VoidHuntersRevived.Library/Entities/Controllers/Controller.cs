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
        
        #endregion

        #region Protected Attributes
        protected IEnumerable<ShipPart> parts => _parts;
        protected ThreadSynchronizer synchronizer { get; private set; }
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

            // this.synchronizer = provider.GetService<ThreadSynchronizer>("synchronizer:controller");
            this.synchronizer = new ThreadSynchronizer();

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

            this.synchronizer.Update(gameTime);
        }
        #endregion

        #region Helper Methods
        protected void TryAdd(ShipPart shipPart)
        {
            this.synchronizer.Do(gt =>
            {
                if (this.CanAdd(shipPart))
                    this.Add(shipPart);
            });
        }

        protected virtual Boolean CanAdd(ShipPart shipPart)
            => shipPart != default(ShipPart) && shipPart.IsRoot;

        protected virtual void Add(ShipPart shipPart)
        {
            shipPart.Controller?.TryRemove(shipPart);
            _parts.Add(shipPart);
            shipPart.Controller = this;
        }

        protected virtual Boolean CanRemove(ShipPart shipPart)
            => _parts.Contains(shipPart);

        protected internal virtual void TryRemove(ShipPart shipPart)
        {
            if (this.CanRemove(shipPart))
                this.Remove(shipPart);
        }

        protected virtual void Remove(ShipPart shipPart)
        {
            this.synchronizer.Do(gt => _parts.Remove(shipPart));


            if (shipPart.IsRoot) // Reset the controller if the part is still root.
                shipPart.Controller = null;
        }
        #endregion
    }
}
