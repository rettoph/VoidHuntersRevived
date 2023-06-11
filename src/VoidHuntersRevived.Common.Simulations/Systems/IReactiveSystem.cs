using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public interface IReactiveSystem<T> : ISystem
        where T : unmanaged
    {
        void OnAdded(in EntityId entityKey, in Ref<T> component);
        void OnRemoved(in EntityId entityKey, in Ref<T> component);
    }
}
