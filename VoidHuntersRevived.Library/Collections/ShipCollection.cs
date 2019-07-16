using Guppy;
using Guppy.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Collections
{
    public class ShipCollection : UniqueObjectCollection<Ship>
    {
        private HashSet<Ship> _availableShips;
        private EntityCollection _entities;

        public ShipCollection(EntityCollection entities) : base(true)
        {
            _entities = entities;
            _availableShips = new HashSet<Ship>();

            _entities.Created += this.HandleEntityCreated;
            _entities.Removed += this.HandleEntityRemoved;
        }

        /// <summary>
        /// Get or create a ship object that has
        /// no player linked to it
        /// </summary>
        /// <returns></returns>
        public Ship GetOrCreateAvailableShip()
        {
            if (_availableShips.Count == 0)
                return _entities.Create<Ship>("entity:ship");
            else
                return _availableShips.First();
        }

        #region Utility Methods
        public override void Add(Ship item)
        {
            base.Add(item);

            item.OnPlayerChanged += this.HandleShipPlayerChanged;
        }

        public override bool Remove(Ship item)
        {
            if(base.Remove(item))
            {
                item.OnPlayerChanged -= this.HandleShipPlayerChanged;

                return true;
            }

            return false;
        }
        #endregion

        #region Event Handlers
        private void HandleShipPlayerChanged(object sender, ChangedEventArgs<Player> e)
        {
            if (e.New == null)
                _availableShips.Add(sender as Ship);
            else if (e.Old == null)
                _availableShips.Remove(sender as Ship);
        }

        private void HandleEntityCreated(object sender, Entity e)
        {
            if (e is Ship)
                this.Add(e as Ship);
        }

        private void HandleEntityRemoved(object sender, Entity e)
        {
            if (e is Ship)
                this.Remove(e as Ship);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            _entities.Created -= this.HandleEntityCreated;
            _entities.Removed -= this.HandleEntityRemoved;
        }
    }
}
