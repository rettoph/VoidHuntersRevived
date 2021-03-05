using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;

namespace VoidHuntersRevived.Library.Entities
{
    public partial class Ship
    {
        #region Private Fields
        /// <summary>
        /// The internal field used for <see cref="Energy"/>.
        /// </summary>
        private Single _energy;

        /// <summary>
        /// A list of all power cells contained within the
        /// Ship's Bridge's chain. This is
        /// automatically updated when <see cref="OnClean"/>
        /// is invoked via the private <see cref="Energy_HandleClean(Ship)"/>
        /// </summary>
        private List<PowerCell> _powerCells = new List<PowerCell>();
        #endregion

        #region Public Properties
        /// <summary>
        /// The current amount of energy within the ship.
        /// </summary>
        public Single Energy
        {
            get => _energy;
            set => this.OnEnergyChanged.InvokeIf(_energy != value, this, ref _energy, value);
        }

        /// <summary>
        /// The maximum amount of energy allowed by the current
        /// ship, combined with all chain power cells.
        /// </summary>
        public Single MaxEnergy { get; private set; }

        /// <summary>
        /// The amount of energy to be charged per second, based on internal
        /// power cells.
        /// </summary>
        public Single EnergyChargePerSecond { get; private set; }

        /// <summary>
        /// The current ratio of <see cref="Energy"/> over
        /// <see cref="MaxEnergy"/>
        /// </summary>
        public Single EnergyPercentage => this.Energy / this.MaxEnergy;

        /// <summary>
        /// Whether or not the current ship is in an
        /// <see cref="Energy"/> charge state.
        /// </summary>
        public Boolean Charging { get; set; }
        #endregion

        #region Events
        public event OnChangedEventDelegate<Ship, Single> OnEnergyChanged;
        #endregion

        #region Lifecycle Methods
        private void Energy_Create(ServiceProvider provider)
        {
            this.OnUpdate += this.Energy_Update;
            this.OnClean += Ship.Energy_HandleClean;
        }

        private void Energy_PreInitialize(ServiceProvider provider)
        {
            this.Energy = this.MaxEnergy;
            this.Charging = false;
        }

        private void Energy_Dispose()
        {
            this.OnUpdate -= this.Energy_Update;
            this.OnClean -= Ship.Energy_HandleClean;
        }
        #endregion

        #region Frame Methods
        private void Energy_Update(GameTime gameTime)
        {
            if (this.Energy < this.MaxEnergy || this.Charging)
            {
                this.Energy += this.EnergyChargePerSecond * (Single)gameTime.ElapsedGameTime.TotalSeconds;

                if(this.Energy > this.MaxEnergy)
                {
                    this.Energy = this.MaxEnergy;
                    this.Charging = false;
                }
            }

        }
        #endregion

        #region Methods
        public Boolean TryUseEnergy(Single amount)
        {
            if (this.Energy <= 0 || this.Charging)
                return false;

            this.Energy -= amount;

            if (this.Energy <= 0)
            {
                this.Energy = -10;
                this.Charging = true;
            }

            return true;
        }
        #endregion

        #region Event Handlers
        private static void Energy_HandleClean(Ship sender)
        {
            lock (sender._powerCells)
            {
                // Refresh the internal list of weapons...
                sender._powerCells.Clear();
                sender._powerCells.AddRange(sender.Bridge?.Items(c => c is PowerCell).Select(c => c as PowerCell) ?? Enumerable.Empty<PowerCell>());

                sender.MaxEnergy = 100f + sender._powerCells.Sum(pc => pc.Context.EnergyCapacity);
                sender.EnergyChargePerSecond = 25f + sender._powerCells.Sum(pc => pc.Context.EnergyChargePerSecond);

                sender.Energy = Math.Min(sender.Energy, sender.MaxEnergy);
            }
        }
        #endregion
    }
}
