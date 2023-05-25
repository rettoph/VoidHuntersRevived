using Guppy.Attributes;
using Guppy.ECS.Loaders;
using Guppy.ECS.Services;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    public sealed class EntityLoader : IEntityLoader
    {
        public void Configure(IEntityService entities)
        {
            entities.Configure(EntityTypes.Ship, configuration =>
            {
                configuration.Inherit(EntityTypes.ShipPart)
                    .AttachComponent(e => new Helm())
                    .AttachComponent(e => new Tactical())
                    .AttachComponent(e => new TractorBeamEmitter())
                    .EnsureComponent<IBody>();
            });
        }
    }
}
