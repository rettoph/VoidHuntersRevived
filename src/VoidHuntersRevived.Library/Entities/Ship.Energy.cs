using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities
{
    public partial class Ship
    {
        #region Private Fields
        private Single _energy;
        #endregion

        #region Public Properties
        public Single Energy
        {
            get => _energy;
            set => this.OnEnergyChanged.InvokeIf(_energy != value, this, ref _energy, value);
        }
        public Single MaxEnergy => 100f;
        public Single EnergyPercentage => this.Energy / this.MaxEnergy;
        public Boolean Charging { get; private set; }
        #endregion

        #region Events
        public event OnChangedEventDelegate<Ship, Single> OnEnergyChanged;
        #endregion

        #region Lifecycle Methods
        private void Energy_Create(ServiceProvider provider)
        {
            this.OnUpdate += this.Energy_Update;
        }

        private void Energy_PreInitialize(ServiceProvider provider)
        {
            this.Energy = this.MaxEnergy;
            this.Charging = false;
        }

        private void Energy_Dispose()
        {
            this.OnUpdate -= this.Energy_Update;
        }
        #endregion

        #region Frame Methods
        private void Energy_Update(GameTime gameTime)
        {
            if (this.Energy < this.MaxEnergy)
            {
                this.Energy += 25f * (Single)gameTime.ElapsedGameTime.TotalSeconds;

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
    }
}
