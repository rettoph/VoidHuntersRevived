using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.ECS.Loaders;
using Guppy.ECS.Services;
using Guppy.Loaders;
using Guppy.Network;
using Guppy.Network.Identity.Claims;
using Guppy.Resources.Serialization.Json;
using Guppy.Resources.Serialization.Json.Converters;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities;
using tainicom.Aether.Physics2D.Dynamics;
using Volatile;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    public sealed class EntityLoader : IEntityLoader
    {
        public void Configure(IEntityService entities)
        {
            entities.Configure(EntityTypes.VoltWorld, configuration =>
            {
                configuration.AttachComponent(e => new VoltWorld());
            });

            entities.Configure(EntityTypes.Ship, configuration =>
            {
                configuration.Inherit(EntityTypes.ShipPart)
                    .AttachComponent(e => new Helm())
                    .AttachComponent(e => new Tactical())
                    .AttachComponent(e => new TractorBeamEmitter())
                    .EnsureComponent<Body>();
            });
        }
    }
}
