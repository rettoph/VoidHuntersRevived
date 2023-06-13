using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Simulations.Abstractions
{
    public struct Component<T> : IEntityComponent
        where T : unmanaged
    {
        public static readonly ComponentBuilder<Component<T>> Builder;

        static Component()
        {
            Builder = new ComponentBuilder<Component<T>>();
        }

        public T Instance;
    }
}
