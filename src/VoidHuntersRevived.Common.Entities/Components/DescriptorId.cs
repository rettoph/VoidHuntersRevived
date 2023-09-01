using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct DescriptorId : IEntityComponent
    {
        public readonly VhId Value;

        public DescriptorId(VhId value)
        {
            Value = value;
        }
    }
}
