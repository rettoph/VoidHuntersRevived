using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Simulations.Services
{
    public delegate void IterateDelegate<T1>(Step step, in EntityId id, ref T1 component1) 
        where T1 : unmanaged;

    public delegate void IterateDelegate<T1, T2>(Step step, in EntityId id, ref T1 component1, ref T2 component2)
        where T1 : unmanaged
        where T2 : unmanaged;

    public delegate void IterateDelegate<T1, T2, T3>(Step step, in EntityId id, ref T1 component1, ref T2 component2, ref T3 component3)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged;

    public delegate void IterateDelegate<T1, T2, T3, T4>(Step step, in EntityId id, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4)
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged;

    public interface IComponentService
    {
        bool TryGet<T1>(EntityId id, out Ref<T1> component1)
            where T1 : unmanaged;

        bool TryGet<T1, T2>(EntityId id, out Ref<T1> component1, out Ref<T2> component2)
            where T1 : unmanaged
            where T2 : unmanaged;

        bool TryGet<T1, T2, T3>(EntityId id, out Ref<T1> component1, out Ref<T2> component2, out Ref<T3> component3)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged;

        bool TryGet<T1, T2, T3, T4>(EntityId id, out Ref<T1> component1, out Ref<T2> component2, out Ref<T3> component3, out Ref<T4> component4)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged;

        void Iterate<T1>(IterateDelegate<T1> iterator, Step step)
            where T1 : unmanaged;

        void Iterate<T1, T2>(IterateDelegate<T1, T2> iterator, Step step)
            where T1 : unmanaged
            where T2 : unmanaged;

        void Iterate<T1, T2, T3>(IterateDelegate<T1, T2, T3> iterator, Step step)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged;

        void Iterate<T1, T2, T3, T4>(IterateDelegate<T1, T2, T3, T4> iterator, Step step)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where T4 : unmanaged;
    }
}
