using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Initializers
{
    [AutoLoad]
    internal sealed class InstanceEntityInitializer : IEntityInitializer
    {
        public IEntityType[] ExplicitEntityTypes => Array.Empty<IEntityType>();

        public int Order => int.MinValue;

        public InstanceEntityInitializer()
        {
        }

        public bool ShouldInitialize(IEntityType entityType)
        {
            return true;
        }

        public InstanceEntityInitializerDelegate? InstanceInitializer(IEntityType entityType)
        {
            var (_, _, instance) = StaticEntityHelper.GetComponents(entityType);

            return (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                initializer.Init(instance);
            };
        }

        public DisposeEntityInitializerDelegate? InstanceDisposer(IEntityType entityType)
        {
            return null;
        }

        public StaticEntityInitializerDelegate? StaticInitializer(IEntityType entityType)
        {
            return null;
        }

        public DisposeEntityInitializerDelegate? StaticDisposer(IEntityType entityType)
        {
            return null;
        }
    }
}
