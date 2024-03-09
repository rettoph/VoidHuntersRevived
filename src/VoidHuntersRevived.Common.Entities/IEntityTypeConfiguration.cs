using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityTypeConfiguration : IDisposable
    {
        public IEntityType Type { get; }

        void InitializeInstance(IEntityService entities, ref EntityInitializer initializer, in EntityId id);
        void InitializeStatic(ref EntityInitializer initializer);

        IEntityTypeConfiguration InitializeInstanceComponent<T>(IEntityTypeComponentInitializer<T> componentInitializer)
            where T : unmanaged, IEntityComponent;

        IEntityTypeConfiguration InitializeStaticComponent<T>(T instance)
            where T : unmanaged, IEntityComponent;
    }
}
