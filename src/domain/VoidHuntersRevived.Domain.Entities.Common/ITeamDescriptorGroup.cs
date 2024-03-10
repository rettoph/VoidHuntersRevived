using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public interface ITeamDescriptorGroup
    {
        ITeam Team { get; }
        VoidHuntersEntityDescriptor Descriptor { get; }
        ExclusiveGroupStruct GroupId { get; }
    }
}
