using Guppy.Network.Identity.Dtos;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Simulations.Common.Events
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
