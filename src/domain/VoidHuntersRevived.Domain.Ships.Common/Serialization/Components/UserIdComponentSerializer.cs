using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Serialization.Components
{
    [AutoLoad]
    public class UserIdComponentSerializer : RawComponentSerializer<UserId>
    {
    }
}
