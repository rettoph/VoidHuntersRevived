using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.Players
{
    /// <summary>
    /// Fighter drones have some specfial functionality (such as decaying life)
    /// that should be handled here.
    /// </summary>
    public class DronePlayer : ComputerPlayer
    {
        #region Public Properties
        /// <summary>
        /// The maximum age of this drone (in seconds)
        /// </summary>
        public Single MaxAge { get; set; } = 45;

        /// <inheritdoc />
        public override bool DestroyOnDeath => true;
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.Ship?.Bridge == default)
                return;

            // Decay health at a ratio matching the max age.
            var lifePercent = (Single)gameTime.ElapsedGameTime.TotalSeconds / this.MaxAge;
            this.Ship.Bridge.TryApplyDamage(this.Ship.Bridge.Context.MaxHealth * lifePercent);
        }
        #endregion
    }
}
