using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.ECS.Loaders;
using VoidHuntersRevived.Common.ECS.Services;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    public sealed class EntityLoader : IEntityTypeLoader
    {
        public void Configure(IEntityTypeService entities)
        {
            entities.Configure(EntityTypes.Ship, configuration =>
            {
                configuration.Has<Helm>();
            });
        }
    }
}
