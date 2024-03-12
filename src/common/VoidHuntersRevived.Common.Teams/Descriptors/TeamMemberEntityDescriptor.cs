using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Teams.Descriptors
{
    public abstract class TeamMemberEntityDescriptor : VoidHuntersEntityDescriptor
    {
        public TeamMemberEntityDescriptor()
        {
            this.WithInstanceComponents([
                new ComponentBuilder<Id<ITeam>>()
            ]);
        }
    }
}
