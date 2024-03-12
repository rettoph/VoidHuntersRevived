using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Teams;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Teams.Common.Services
{
    public interface ITeamDescriptorGroupService
    {
        ITeamDescriptorGroup GetByGroupId(ExclusiveGroupStruct groupId);

        Dictionary<Id<ITeam>, ITeamDescriptorGroup> GetAllByDescriptor(VoidHuntersEntityDescriptor descriptor);

        Dictionary<Id<ITeam>, ITeamDescriptorGroup[]> GetAllWithComponentsByTeams(params Type[] components);
    }
}
