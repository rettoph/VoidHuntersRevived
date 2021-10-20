using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;

namespace VoidHuntersRevived.Library.Contexts.ShipParts
{
    [ShipPartContextType("Thruster")]
    public class ThrusterContext : ShipPartContext
    {
        protected override ServiceConfigurationKey shipPartServiceConfigurationKey => Constants.ServiceConfigurationKeys.ShipParts.Thruster;

        public Vector2 MaximumThrust { get; set; }
    }
}
