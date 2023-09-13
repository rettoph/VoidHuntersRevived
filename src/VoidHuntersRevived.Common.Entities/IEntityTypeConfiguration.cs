using Svelto.ECS;
using Svelto.ECS.Serialization;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityTypeConfiguration : IDisposable
    {
        public IEntityType Type { get; }

        void Initialize(IEntityService entities, ref EntityInitializer initializer, in EntityId id);

        IEntityTypeConfiguration InitializeComponent<T>(IEntityTypeComponentValue<T> componentInitializer)
            where T : unmanaged, IEntityComponent;
    }
}
