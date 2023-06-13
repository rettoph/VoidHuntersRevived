using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public delegate void IterateDelegate<T1>(Step step, in Guid id, ref T1 component1) 
        where T1 : unmanaged, IEntityComponent;

    public delegate void IterateDelegate<T1, T2>(Step step, in Guid id, ref T1 component1, ref T2 component2)
        where T1 : unmanaged, IEntityComponent
        where T2 : unmanaged, IEntityComponent;

    public delegate void IterateDelegate<T1, T2, T3>(Step step, in Guid id, ref T1 component1, ref T2 component2, ref T3 component3)
        where T1 : unmanaged, IEntityComponent
        where T2 : unmanaged, IEntityComponent
        where T3 : unmanaged, IEntityComponent;

    public delegate void IterateDelegate<T1, T2, T3, T4>(Step step, in Guid id, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4)
        where T1 : unmanaged, IEntityComponent
        where T2 : unmanaged, IEntityComponent
        where T3 : unmanaged, IEntityComponent
        where T4 : unmanaged, IEntityComponent;

    public interface IComponentService
    {
        bool TryGet<T1>(Guid id, out Ref<T1> component1)
            where T1 : unmanaged, IEntityComponent;

        bool TryGet<T1, T2>(Guid id, out Ref<T1> component1, out Ref<T2> component2)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent;

        bool TryGet<T1, T2, T3>(Guid id, out Ref<T1> component1, out Ref<T2> component2, out Ref<T3> component3)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent;

        bool TryGet<T1, T2, T3, T4>(Guid id, out Ref<T1> component1, out Ref<T2> component2, out Ref<T3> component3, out Ref<T4> component4)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent;

        void Iterate<T1>(IterateDelegate<T1> iterator, Step step)
            where T1 : unmanaged, IEntityComponent;

        void Iterate<T1, T2>(IterateDelegate<T1, T2> iterator, Step step)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent;

        void Iterate<T1, T2, T3>(IterateDelegate<T1, T2, T3> iterator, Step step)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent;

        void Iterate<T1, T2, T3, T4>(IterateDelegate<T1, T2, T3, T4> iterator, Step step)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent;
    }
}
