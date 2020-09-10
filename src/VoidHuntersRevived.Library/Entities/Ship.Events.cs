using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Events;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Partial class to manage all internal Ship
    /// events & event calling.
    /// </summary>
    public partial class Ship
    {
        #region Events
        public event ShipEventManager.ShipEventDelegate OnEvent;

        public Dictionary<ShipEventType, ShipEventManager> Events { get; private set; }
        #endregion

        #region Lifecycle Methods
        private void Events_PreIninitialize(ServiceProvider provider)
        {
            this.Events = ((ShipEventType[])Enum.GetValues(typeof(ShipEventType))).ToDictionary(
                keySelector: set => set,
                elementSelector: set => new ShipEventManager(this, set));
        }

        private void Events_Dispose()
        {
            this.Events.Clear();
        }
        #endregion

        #region Helper Methods
        public Boolean TryInvokeEvent(ShipEventArgs args)
        {
            if (!this.Events[args.Type].TryInvoke(args))
                return false;

            this.OnEvent?.Invoke(this, args);

            return true;
        }
        #endregion
    }
}
