using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Common.Components;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Pieces.Components;

namespace VoidHuntersRevived.Game.Loaders
{
    [AutoLoad]
    public sealed class EntityLoader : IEntityTypeLoader, IEntityLoader
    {
        public void Configure(IEntityTypeService entityTypes)
        {
            entityTypes.Configure(EntityTypes.Ship, configuration =>
            {
                configuration
                    .HasComponent<Helm>()
                    .HasComponent<Tree>()
                    .HasComponent<Body>();
            });
        }

        public void Configure(IEntityConfigurationService entities)
        {
            entities.Configure(EntityNames.UserShip, configuration =>
            {
                configuration.SetType(EntityTypes.Ship);
            });
        }
    }
}
