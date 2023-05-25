using Guppy.Attributes;
using Guppy.ECS.Loaders;
using Guppy.ECS.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Domain.Entities.ShipParts.Loaders
{
    [AutoLoad]
    public sealed class ShipPartEntityLoader : IEntityLoader
    {
        public void Configure(IEntityService entities)
        {
            entities.Configure(EntityTypes.ShipPart, configuration =>
            {
                configuration.EnsureComponent<ShipPart>();
            });
        }
    }
}
