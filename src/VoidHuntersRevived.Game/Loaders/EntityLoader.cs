using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Common.Components;
using VoidHuntersRevived.Game.Components;

namespace VoidHuntersRevived.Game.Loaders
{
    [AutoLoad]
    public sealed class EntityLoader : IEntityTypeLoader, IEntityLoader
    {
        public void Configure(IEntityTypeService entities)
        {
            entities.Configure(EntityTypes.Ship, configuration =>
            {
                configuration
                    .HasComponent<Helm>()
                    .HasComponent<Body>()
                    .HasComponent<Tree>();
            });
        }

        public void Configure(IEntityConfigurationService configuration)
        {
            configuration.Configure(EntityNames.UserShip, configuration =>
            {
                configuration.SetType(EntityTypes.Ship);
            });
        }
    }
}
