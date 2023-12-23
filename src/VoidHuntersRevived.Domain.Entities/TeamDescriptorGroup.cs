using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Domain.Entities
{
    internal sealed class TeamDescriptorGroup : ITeamDescriptorGroup
    {
        public ITeam Team { get; }

        public VoidHuntersEntityDescriptor Descriptor { get; }

        public ExclusiveGroupStruct GroupId { get; }

        public Color PrimaryColor { get; }
        public Color SecondaryColor { get; }

        public TeamDescriptorGroup(ITeam team, VoidHuntersEntityDescriptor descriptor, ExclusiveGroupStruct groupId, IResourceProvider resources)
        {
            Team = team;
            Descriptor = descriptor;
            GroupId = groupId;

            PrimaryColor = Team.PrimaryColor == default ? resources.Get(Descriptor.PrimaryColor) : Team.PrimaryColor;
            SecondaryColor = Team.SecondaryColor == default ? resources.Get(Descriptor.SecondaryColor) : Team.SecondaryColor;
        }
    }
}
