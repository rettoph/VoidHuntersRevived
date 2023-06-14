using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    public struct EntityVhId : IEntityComponent
    {
        public VhId Value;
    }
}
