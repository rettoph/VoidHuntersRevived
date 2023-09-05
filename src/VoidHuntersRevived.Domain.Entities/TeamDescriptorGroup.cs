using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Domain.Entities
{
    internal sealed class TeamDescriptorGroup : ITeamDescriptorGroup
    {
        public ITeam Team { get; }

        public VoidHuntersEntityDescriptor Descriptor { get; }

        public ExclusiveGroupStruct GroupId { get; }

        public Color Color { get; }

        public TeamDescriptorGroup(ITeam team, VoidHuntersEntityDescriptor descriptor, ExclusiveGroupStruct groupId, IResourceProvider resources)
        {
            Team = team;
            Descriptor = descriptor;
            GroupId = groupId;

            Color = Team.Color == default ? resources.Get(Descriptor.DefaultColor) : Team.Color;
        }
    }
}
