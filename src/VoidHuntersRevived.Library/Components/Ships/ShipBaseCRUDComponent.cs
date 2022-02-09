using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Globals.Constants;

namespace VoidHuntersRevived.Library.Components.Ships
{
    [HostTypeRequired(HostType.Remote)]
    internal class ShipBaseCRUDComponent : Component<Ship>
    {
    }
}
