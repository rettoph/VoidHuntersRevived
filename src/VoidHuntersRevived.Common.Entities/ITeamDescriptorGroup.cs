using Microsoft.Xna.Framework;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities
{
    public interface ITeamDescriptorGroup
    {
        ITeam Team { get; }
        VoidHuntersEntityDescriptor Descriptor { get; }
        ExclusiveGroupStruct GroupId { get; }

        Color Color { get; }
    }
}
