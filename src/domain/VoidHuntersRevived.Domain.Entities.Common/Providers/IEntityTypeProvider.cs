using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Providers
{
    public interface IEntityTypeProvider : IDisposable
    {
        public IEntityType Type { get; }

        void InitializeInstance(IEntityService entities, IEntityType type, in EntityId id, ref EntityInitializer initializer);
        void InitializeType(IEntityService entities, IEntityType type, in EntityId id, ref EntityInitializer initializer);
    }
}
