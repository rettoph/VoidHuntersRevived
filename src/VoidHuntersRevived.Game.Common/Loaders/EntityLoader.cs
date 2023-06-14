using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Common.Components;

namespace VoidHuntersRevived.Game.Entities.Loaders
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
