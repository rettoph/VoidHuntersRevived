using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Teams;

namespace VoidHuntersRevived.Domain.Teams.Common
{
    public interface ITeamDescriptorGroup
    {
        ITeam Team { get; }
        VoidHuntersEntityDescriptor Descriptor { get; }
        ExclusiveGroupStruct GroupId { get; }
    }
}
