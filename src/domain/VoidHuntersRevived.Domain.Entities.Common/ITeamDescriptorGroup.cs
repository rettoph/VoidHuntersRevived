using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities
{
    public interface ITeamDescriptorGroup
    {
        ITeam Team { get; }
        VoidHuntersEntityDescriptor Descriptor { get; }
        ExclusiveGroupStruct GroupId { get; }
    }
}
