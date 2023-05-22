using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Lockstep.Providers;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;
using VoidHuntersRevived.Domain.Simulations.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Providers
{
    [PeerTypeFilter(PeerType.Client)]
    internal class ClientTickProvider : DefaultTickProvider
    {

    }
}
