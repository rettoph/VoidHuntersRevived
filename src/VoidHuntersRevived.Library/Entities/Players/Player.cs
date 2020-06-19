using Guppy;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Collections;

namespace VoidHuntersRevived.Library.Entities.Players
{
    /// <summary>
    /// The base player class. By defualt, there are 2 main types
    /// of players. Human and Computers. Players are classes
    /// that can take control of a ship instance.
    /// </summary>
    public abstract class Player : NetworkEntity
    {
        #region Private Fields
        private Ship _ship;
        #endregion

        #region Public Attributes
        public abstract String Name { get; }
        public Ship Ship
        {
            get => _ship;
            set
            {
                if(this.Ship != value)
                {
                    var old = _ship;
                    _ship = value;
                    _ship.Player = this;
                    this.OnShipChanged?.Invoke(this, old, this.Ship);
                }
            }
        }
        #endregion

        #region Events
        public event GuppyDeltaEventHandler<Player, Ship> OnShipChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            // Auto add the current player to the global player collection...
            provider.GetService<PlayerCollection>().TryAdd(this);
        }
        #endregion
    }
}
