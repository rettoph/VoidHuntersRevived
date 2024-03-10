using Guppy.Resources.Providers;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Entities
{
    internal sealed class TeamDescriptorGroup : ITeamDescriptorGroup
    {
        public ITeam Team { get; }

        public VoidHuntersEntityDescriptor Descriptor { get; }

        public ExclusiveGroupStruct GroupId { get; }

        public TeamDescriptorGroup(ITeam team, VoidHuntersEntityDescriptor descriptor, ExclusiveGroupStruct groupId, IResourceProvider resources)
        {
            this.Team = team;
            this.Descriptor = descriptor;
            this.GroupId = groupId;
        }
    }
}
