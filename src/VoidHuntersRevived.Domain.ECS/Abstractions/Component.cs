using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.ECS.Abstractions
{
    public struct Component<T> : IEntityComponent
        where T : unmanaged
    {
        public static readonly ComponentBuilder<Component<T>> Builder = new ComponentBuilder<Component<T>>();

        public T Instance;
    }
}
