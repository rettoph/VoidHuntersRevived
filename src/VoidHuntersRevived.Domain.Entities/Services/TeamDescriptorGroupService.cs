using Guppy.Resources.Providers;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class TeamDescriptorGroupService : ITeamDescriptorGroupService
    {
        private Dictionary<ExclusiveGroupStruct, ITeamDescriptorGroup> _teamDescriptorGroupsByGroupId;
        private Dictionary<TeamId, ITeamDescriptorGroup[]> _teamDescriptorGroupsByTeam;

        public TeamDescriptorGroupService(IResourceProvider resources, ITeamService teams, IEnumerable<VoidHuntersEntityDescriptor> descriptors)
        {
            _teamDescriptorGroupsByGroupId = new Dictionary<ExclusiveGroupStruct, ITeamDescriptorGroup>();

            foreach(VoidHuntersEntityDescriptor descriptor in descriptors)
            {
                foreach(ITeam team in teams.All())
                {
                    ExclusiveGroupStruct groupId = GetExclusiveGroupStruct(descriptor, team);

                    _teamDescriptorGroupsByGroupId.Add(groupId, new TeamDescriptorGroup(team, descriptor, groupId, resources));
                }
            }

            _teamDescriptorGroupsByTeam = _teamDescriptorGroupsByGroupId.Values.GroupBy(x => x.Team.Id).ToDictionary(x => x.Key, x => x.ToArray());
        }

        public Dictionary<TeamId, ITeamDescriptorGroup> GetAllByDescriptor(VoidHuntersEntityDescriptor descriptor)
        {
            return _teamDescriptorGroupsByGroupId.Values.Where(x => x.Descriptor == descriptor).ToDictionary(x => x.Team.Id, x => x);
        }

        public ITeamDescriptorGroup GetByGroupId(ExclusiveGroupStruct groupId)
        {
            return _teamDescriptorGroupsByGroupId[groupId];
        }

        private static ExclusiveGroupStruct GetExclusiveGroupStruct(VoidHuntersEntityDescriptor descriptor, ITeam team)
        {
            string groupName = $"{descriptor.Name}.{team.Name}";
            try
            {
                return ExclusiveGroup.Search(groupName);
            }
            catch
            {
                return new ExclusiveGroup(groupName);
            }
        }

        public Dictionary<TeamId, ITeamDescriptorGroup[]> GetAllWithComponentsByTeams(params Type[] components)
        {
            return _teamDescriptorGroupsByTeam.ToDictionary(
                keySelector: x => x.Key,
                elementSelector: x => x.Value.Where(d => d.Descriptor.HasAll(components)).OrderBy(x => x.Descriptor.Order).ToArray());
        }
    }
}
