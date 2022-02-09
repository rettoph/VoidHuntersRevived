using Microsoft.Xna.Framework;
using System;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Library.Contexts.ShipParts
{
    [ShipPartContextType("Thruster")]
    public class ThrusterContext : ShipPartContext
    {
        protected override String shipPartServiceConfigurationName => typeof(Thruster).FullName;

        public Vector2 Thrust { get; set; }
    }
}
