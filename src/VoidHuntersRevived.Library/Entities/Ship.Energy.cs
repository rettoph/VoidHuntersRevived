using Guppy.DependencyInjection;
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
        public Single Energy => _energy;
        public Single MaxEnergy => 100f;
        public Single EnergyPercentage => this.Energy / this.MaxEnergy;
        public Boolean Charging { get; private set; }
        #endregion

        #region Lifecycle Methods
        private void Energy_Create(ServiceProvider provider)
        {
            this.OnUpdate += this.Energy_Update;
        }

        private void Energy_PreInitialize(ServiceProvider provider)
        {
            _energy = this.MaxEnergy;
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
            if (_energy < this.MaxEnergy)
            {
                _energy += 50f * (Single)gameTime.ElapsedGameTime.TotalSeconds;

                if(_energy > this.MaxEnergy)
                {
                    _energy = this.MaxEnergy;
                    this.Charging = false;
                }
            }

        }
        #endregion

        #region Methods
        public Boolean TryUseEnergy(Single amount)
        {
            if (_energy <= 0 || this.Charging)
                return false;

            _energy -= amount;

            if (_energy <= 0)
            {
                _energy = -10;
                this.Charging = true;
            }

            return true;
        }
        #endregion
    }
}
