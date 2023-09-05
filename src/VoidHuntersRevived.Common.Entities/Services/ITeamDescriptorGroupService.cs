using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface ITeamDescriptorGroupService
    {
        ITeamDescriptorGroup GetByGroupId(ExclusiveGroupStruct groupId);

        Dictionary<TeamId, ITeamDescriptorGroup> GetAllByDescriptor(VoidHuntersEntityDescriptor descriptor);

        Dictionary<TeamId, ITeamDescriptorGroup[]> GetAllWithComponentsByTeams(params Type[] components);
    }
}
