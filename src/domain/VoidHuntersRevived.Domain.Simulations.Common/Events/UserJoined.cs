using Guppy.Core.Network.Common.Dtos;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Simulations.Common.Events
{
    public class UserJoined : IStepInput<UserJoined>
    {
        public bool IsPredictable => false;

        public required UserDto UserDto { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<UserJoined, int>.Instance.Calculate(this.UserDto.Id);
        }
    }
}