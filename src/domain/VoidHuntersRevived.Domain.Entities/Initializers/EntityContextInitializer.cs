using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Initializers
{
    [AutoLoad]
    internal class EntityContextInitializer : BaseEntityInitializer
    {
        public EntityContextInitializer(IEntityContextService contexts)
        {
            this.Order = -1;

            foreach (EntityContext context in contexts.All())
            {
                StaticEntityInitializerDelegate? staticInitializer = EntityInitializerHelper.BuildStaticEntityInitializerDelegate(context.StaticComponents.Values);
                InstanceEntityInitializerDelegate? instanceInitializer = (ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init(context.Id);
                };

                instanceInitializer += EntityInitializerHelper.BuildInstanceEntityInitializerDelegate(context.InstanceComponents.Values);

                this.WithStaticInitializer(context.EntityType, staticInitializer);
                this.WithInstanceInitializer(context.EntityType, instanceInitializer);
            }
        }
    }
}
