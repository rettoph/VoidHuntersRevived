namespace VoidHuntersRevived.Common.Entities.Systems
{
    public interface IReactiveSystem<T> : ISystem
        where T : unmanaged
    {
        void OnAdded(in Guid id, in Ref<T> component);
        void OnRemoved(in Guid id, in Ref<T> component);
    }
}
