using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
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
            var data = StaticEntityHelper.GetData(entityType);

            return (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                initializer.Init(data.InstanceComponent);
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
