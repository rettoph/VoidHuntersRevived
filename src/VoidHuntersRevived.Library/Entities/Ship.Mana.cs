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
        /// A list of all power cells contained within the
        /// Ship's Bridge's chain. This is
        /// automatically updated when <see cref="OnClean"/>
        /// is invoked via the private <see cref="Mana_HandleClean(Ship)"/>
        /// </summary>
        private List<PowerCell> _powerCells = new List<PowerCell>();
        #endregion

        #region Public Properties
        /// <summary>
        /// The current ratio of <see cref="Mana"/> over
        /// <see cref="MaxMana"/>
        /// </summary>
        public Single ManaPercentage => this.Mana / this.MaxMana;
        #endregion

        #region Lifecycle Methods
        private void Mana_Create(ServiceProvider provider)
        {
            this.OnClean += Ship.Mana_HandleClean;
        }

        private void Mana_Dispose()
        {
            this.OnClean -= Ship.Mana_HandleClean;
        }
        #endregion


        #region Event Handlers
        private static void Mana_HandleClean(Ship sender)
        {
            lock (sender._powerCells)
            {
                // Refresh the internal list of weapons...
                sender._powerCells.Clear();
                sender._powerCells.AddRange(sender.Bridge?.Items(c => c is PowerCell).Select(c => c as PowerCell) ?? Enumerable.Empty<PowerCell>());

                sender.MaxMana = 100f + sender._powerCells.Where(pc => pc.Powered).Sum(pc => pc.Context.ManaCapacity);
                sender.ManaPerSecond = 25f + sender._powerCells.Where(pc => pc.Powered).Sum(pc => pc.Context.ManaPerSecond);

                sender.Mana = Math.Min(sender.Mana, sender.MaxMana);
            }
        }
        #endregion
    }
}
