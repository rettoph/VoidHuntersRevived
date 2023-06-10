using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.ECS
{
    public interface IEntityInitializer
    {
        EntityId Key { get; }
        EntityType Type { get; }

        bool Has<T>()
            where T : unmanaged;

        void Set<T>(in T component)
            where T : unmanaged;

        ref T Get<T>()
            where T : unmanaged;
    }
}
