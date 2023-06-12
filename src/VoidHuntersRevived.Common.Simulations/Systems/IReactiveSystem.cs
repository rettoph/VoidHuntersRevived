namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public interface IReactiveSystem<T> : ISystem
        where T : unmanaged
    {
        void OnAdded(in Guid entityKey, in Ref<T> component);
        void OnRemoved(in Guid entityKey, in Ref<T> component);
    }
}
