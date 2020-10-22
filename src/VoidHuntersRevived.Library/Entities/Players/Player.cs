using Guppy;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.DependencyInjection;
using Guppy.Lists;
using Guppy.Events.Delegates;

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
            set => this.OnShipChanged.InvokeIfChanged(value != _ship, this, ref _ship, value);
        }
        #endregion

        #region Events
        public event OnChangedEventDelegate<Player, Ship> OnShipChanged;
        #endregion
    }
}
