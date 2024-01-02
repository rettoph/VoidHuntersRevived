﻿using Guppy.Network.Identity.Claims;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;

namespace VoidHuntersRevived.Common.Simulations.Events
{
    public sealed class UserJoined : IInputData
    {
        public bool IsPredictable => false;

        public VhId ShipVhId => default!;

        public required int UserId { get; init; }
        public required Claim[] Claims { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<UserJoined, int>.Instance.Calculate(this.UserId);
        }
    }
}
