using Guppy.Network.Identity.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Simulations.Events
{
    public sealed class UserJoined : IInputData
    {
        public VhId ShipVhId => throw new NotImplementedException();

        public required int UserId { get; init; }
        public required Claim[] Claims { get; init; }
    }
}
