using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public struct Component<T>
        where T : unmanaged, IEntityComponent
    {
        public readonly EntityId Id;
        public readonly T Value;

        public Component(EntityId id, T value)
        {
            this.Id = id;
            this.Value = value;
        }
    }
}
