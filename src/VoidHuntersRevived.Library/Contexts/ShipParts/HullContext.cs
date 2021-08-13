using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;

namespace VoidHuntersRevived.Library.Contexts.ShipParts
{
    public class HullContext : ShipPartContext
    {
        protected override ServiceConfigurationKey shipPartServiceConfigurationKey => Constants.ServiceConfigurationKeys.ShipParts.Hull;
    }
}
