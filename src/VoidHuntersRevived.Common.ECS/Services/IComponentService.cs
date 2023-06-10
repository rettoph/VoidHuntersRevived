using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.ECS.Services
{
    public interface IComponentService
    {
        public bool TryGet<T1>(EntityId id, out Ref<T1> component1)
            where T1 : unmanaged;

        public bool TryGet<T1, T2>(EntityId id, out Ref<T1> component1, out Ref<T2> component2)
            where T1 : unmanaged
            where T2 : unmanaged;

        public bool TryGet<T1, T2, T3>(EntityId id, out Ref<T1> component1, out Ref<T2> component2, out Ref<T3> component3)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged;

        public bool TryGet<T1, T2, T3, T4>(EntityId id, out Ref<T1> component1, out Ref<T2> component2, out Ref<T3> component3, out Ref<T4> component4)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged;
    }
}
