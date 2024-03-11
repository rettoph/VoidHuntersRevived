using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Initializers
{
    public interface IEntityTypeInitializer : IDisposable
    {
        public IEntityType Type { get; }

        void InitializeInstance(IEntityService entities, ref EntityInitializer initializer, in EntityId id);
        void InitializeStatic(ref EntityInitializer initializer);
    }
}
