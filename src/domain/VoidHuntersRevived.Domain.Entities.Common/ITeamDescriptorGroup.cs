using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public interface ITeamDescriptorGroup
    {
        ITeam Team { get; }
        VoidHuntersEntityDescriptor Descriptor { get; }
        ExclusiveGroupStruct GroupId { get; }
    }
}
