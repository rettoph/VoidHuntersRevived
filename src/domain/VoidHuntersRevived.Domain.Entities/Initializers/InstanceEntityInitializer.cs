using Guppy.Core.Common.Attributes;
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

        public EntityInitializerDelegate? InstanceInitializer(IEntityType entityType)
        {
            var data = EntityTypeHelper.GetData(entityType);

            return (IEntityService _, IEntityType _, ref EntityInitializer initializer, in EntityId _) =>
            {
                initializer.Init(data.InstanceComponent);
            };
        }

        public DisposeEntityInitializerDelegate? InstanceDisposer(IEntityType entityType)
        {
            return null;
        }

        public EntityInitializerDelegate? TypeInitializer(IEntityType entityType)
        {
            return null;
        }

        public DisposeEntityInitializerDelegate? TypeDisposer(IEntityType entityType)
        {
            return null;
        }
    }
}
