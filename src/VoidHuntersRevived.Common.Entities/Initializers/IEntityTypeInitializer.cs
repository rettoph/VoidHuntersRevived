using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Initializers
{
    public interface IEntityTypeInitializer : IDisposable
    {
        public IEntityType Type { get; }

        void InitializeInstance(IEntityService entities, ref EntityInitializer initializer, in EntityId id);
        void InitializeStatic(ref EntityInitializer initializer);
    }
}
