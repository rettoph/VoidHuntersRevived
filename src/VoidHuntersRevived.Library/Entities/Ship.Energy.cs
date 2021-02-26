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

        #region Lifecycle Methods
        private void Energy_Create(ServiceProvider provider)
        {
            this.OnUpdate += this.Energy_Update;
        }

        private void Energy_PreInitialize(ServiceProvider provider)
        {
            _energy = this.GetMaxEnergy();
        }

        private void Energy_Dispose()
        {
            this.OnUpdate -= this.Energy_Update;
        }
        #endregion

        #region Frame Methods
        private void Energy_Update(GameTime gameTime)
        {
            if (_energy < this.GetMaxEnergy())
                _energy += 50f * (Single)gameTime.ElapsedGameTime.TotalSeconds;
        }
        #endregion

        #region Methods
        public Single GetEnergy()
        {
            return _energy;
        }

        public Single GetMaxEnergy()
        {
            return 100f;
        }

        public Boolean TryUseEnergy(Single amount)
        {
            if (_energy <= 0)
                return false;

            _energy -= amount;

            return true;
        }
        #endregion
    }
}
