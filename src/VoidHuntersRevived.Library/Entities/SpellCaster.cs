using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities
{
    public class SpellCaster : NetworkEntity
    {
        #region Private Properties
        private Single _mana;
        private Boolean _charging;
        #endregion

        #region Public Properties
        /// <summary>
        /// An internal value of mana the spellcaster currently contains.
        /// </summary>
        public Single Mana
        {
            get => _mana;
            set => this.OnManaChanged.InvokeIf(_mana != value, this, ref _mana, value);
        }

        /// <summary>
        /// The maximum amount of mana the current spellcaster can 
        /// hold.
        /// </summary>
        public Single MaxMana { get; set; }

        /// <summary>
        /// The amount of mana gained internally per second
        /// (capped at the maximum).
        /// </summary>
        public Single ManaPerSecond { get; set; }

        /// <summary>
        /// Whether or not the current <see cref="SpellCaster.Mana"/>
        /// is in a charge state.
        /// </summary>
        public Boolean Charging
        {
            get => _charging;
            set => this.OnChargingChanged.InvokeIf(value != _charging, this, ref _charging, value);
        }
        #endregion

        #region Events
        public OnChangedEventDelegate<SpellCaster, Single> OnManaChanged;
        public OnChangedEventDelegate<SpellCaster, Boolean> OnChargingChanged;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Mana = this.MaxMana;
            this.Charging = false;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.Mana < this.MaxMana || this.Charging)
            {
                this.Mana += this.ManaPerSecond * (Single)gameTime.ElapsedGameTime.TotalSeconds;

                if (this.Mana > this.MaxMana)
                {
                    this.Mana = this.MaxMana;
                    this.Charging = false;
                }
            }
        }
        #endregion

        #region API Methods
        /// <summary>
        /// Determines whether or not a mana consumption is 
        /// possible.
        /// </summary>
        /// <param name="mana"></param>
        /// <returns></returns>
        public Boolean CanConsumeMana(Single mana)
            => !this.Charging;

        /// <summary>
        /// Attempt to consume the requested amount of mana.
        /// </summary>
        /// <param name="mana"></param>
        /// <returns></returns>
        public Boolean TryConsumeMana(Single mana)
        {
            if(this.CanConsumeMana(mana))
            {
                this.Mana -= mana;

                if(this.Mana <= 0)
                {
                    this.Charging = true;
                    this.Mana = 0;
                }

                return true;
            }

            return false;
        }
        #endregion
    }
}
