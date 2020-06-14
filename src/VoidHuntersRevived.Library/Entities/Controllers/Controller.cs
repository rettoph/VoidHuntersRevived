using Guppy;
using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

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
        #endregion

        #region Protected Attributes
        protected IEnumerable<ShipPart> parts => _parts;
        #endregion

        #region Public Attributes
        /// <summary>
        /// Used to determin if a ShipPart should behave as a slave
        /// or master when contained within the current controller.
        /// </summary>
        public GameAuthorization Authorization { get; protected set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _parts = new HashSet<ShipPart>();

            this.Authorization = provider.GetService<Settings>().Get<GameAuthorization>();
        }

        protected override void Dispose()
        {
            base.Dispose();

            _parts.Clear();
        }
        #endregion

        #region Helper Methods
        protected virtual Boolean CanAdd(ShipPart shipPart)
            => shipPart != default(ShipPart) && shipPart.IsRoot;

        protected virtual void Add(ShipPart shipPart)
        {
            shipPart.Controller?.Remove(shipPart);
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
            _parts.Remove(shipPart);

            if(shipPart.IsRoot) // Reset the controller if the part is still root.
                shipPart.Controller = null;
        }
        #endregion
    }
}
