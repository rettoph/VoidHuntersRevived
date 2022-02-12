using Guppy.EntityComponent.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Globals.Constants;

namespace VoidHuntersRevived.Library.Contexts.ShipParts
{
    [ShipPartContextType("Hull")]
    public class HullContext : ShipPartContext
    {
        protected override Type shipPartServiceConfigurationType => typeof(Hull);
    }
}
