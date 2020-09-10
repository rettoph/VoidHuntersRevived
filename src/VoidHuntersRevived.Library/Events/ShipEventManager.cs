using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Events
{
    public sealed class ShipEventManager
    {
        public delegate Boolean ValidateShipEventDelegate(Ship ship, ShipEventArgs args);
        public delegate void ShipEventDelegate(Ship ship, ShipEventArgs args);

        public event ValidateShipEventDelegate ValidateEvent;
        public event ShipEventDelegate OnEvent;

        public readonly Ship Ship;
        public readonly ShipEventType Type;

        internal ShipEventManager(Ship ship, ShipEventType type)
        {
            this.Ship = ship;
            this.Type = type;
        }

        internal Boolean TryInvoke(ShipEventArgs args)
        {
            if (!this.ValidateEvent?.Invoke(this.Ship, args) ?? false)
                return false;

            this.OnEvent?.Invoke(this.Ship, args);

            return true;
        }
    }
}
