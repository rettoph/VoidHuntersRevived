using Guppy.Network.Identity.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Events
{
    public sealed class UserJoined
    {
        public required int UserId { get; init; }
        public required Claim[] Claims { get; init; }
    }
}
