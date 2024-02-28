using Guppy.Network.Identity.Dtos;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;

namespace VoidHuntersRevived.Common.Simulations.Events
{
    public sealed class UserJoined : IInputData
    {
        public bool IsPredictable => false;

        public VhId ShipVhId => default!;

        public required UserDto UserDto { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<UserJoined, int>.Instance.Calculate(this.UserDto.Id);
        }
    }
}
