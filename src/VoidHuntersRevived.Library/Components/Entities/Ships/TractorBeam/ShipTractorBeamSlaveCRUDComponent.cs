using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class ShipTractorBeamSlaveCRUDComponent : ShipTractorBeamComponent
    {
    }
}
