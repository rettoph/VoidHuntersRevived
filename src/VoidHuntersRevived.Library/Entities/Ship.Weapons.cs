using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Partial class containing <see cref="Weapon"/> specific functionality
    /// for <see cref="Ship"/> instances.
    /// </summary>
    public partial class Ship
    {
        #region Private Fields
        /// <summary>
        /// Determin whether or not the current ship
        /// is in the  act of firing.
        /// </summary>
        private Boolean _firing;

        /// <summary>
        /// A list of all weapons contained within the
        /// Ship's Bridge's chain. This is
        /// automatically updated when <see cref="OnClean"/>
        /// is invoked via the private <see cref="Weapons_HandleClean(Ship)"/>
        /// handler.
        /// </summary>
        private List<Weapon> _weapons;
        #endregion

        #region Public Properties
        /// <summary>
        /// Determin whether or not the current ship
        /// is in the  act of firing. Updating this value will automatically invoked
        /// the <see cref="OnFiringChanged"/> event.
        /// </summary>
        public Boolean Firing
        {
            get => _firing;
            set => this.OnFiringChanged.InvokeIfChanged(value != _firing, this, ref _firing, value);
        }

        /// <summary>
        /// A readonly collection of all weapons contained within the
        /// Ship's Bridge's chain. Value based on the
        /// private <see cref="_weapons"/> field.
        /// </summary>
        public IReadOnlyCollection<Weapon> Weapons => _weapons;
        #endregion

        #region Events
        /// <summary>
        /// Automatically invoked when the <see cref="Firing"/> property is updated.
        /// </summary>
        public event OnEventDelegate<Ship, Boolean> OnFiringChanged;
        #endregion

        #region Lifecycle Methods
        private void Weapons_Create(ServiceProvider provider)
        {
            this.OnUpdate += this.Weapons_Update;
            this.OnClean += Ship.Weapons_HandleClean;
        }

        private void Weapons_Dispose()
        {
            this.OnUpdate -= this.Weapons_Update;
            this.OnClean -= Ship.Weapons_HandleClean;
        }
        #endregion

        #region Frame Methods
        private void Weapons_Update(GameTime gameTime)
        {
            if (this.Firing) // Update the weapon & fire it...
                _weapons.ForEach(w =>
                {
                    w.TryUpdate(gameTime);
                    w.TryFire(gameTime);
                });
            else // Just update the weapon...
                _weapons.ForEach(w => w.TryUpdate(gameTime));
        }
        #endregion

        #region Event Handlers
        private static void Weapons_HandleClean(Ship sender)
        {
            lock (sender._weapons)
            {
                // Refresh the internal list of weapons...
                sender._weapons.Clear();
                sender._weapons.AddRange(sender.Bridge?.Items(c => c is Weapon).Select(c => c as Weapon));
            }
        }
        #endregion
    }
}
