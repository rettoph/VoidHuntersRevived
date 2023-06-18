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
            entityTypes.Configure(EntityTypes.Tree, configuration =>
            {
                configuration
                    .HasComponent<Body>()
                    .HasComponent<Tree>();
            });

            entityTypes.Configure(EntityTypes.Ship, configuration =>
            {
                configuration.Inherits(EntityTypes.Tree)
                    .HasComponent<Helm>();
            });
        }

        public void Configure(IEntityConfigurationService entities)
        {
            entities.Configure(EntityNames.Chain, configuration =>
            {
                configuration.SetType(EntityTypes.Tree);
            });

            entities.Configure(EntityNames.UserShip, configuration =>
            {
                configuration.SetType(EntityTypes.Ship);
            });
        }
    }
}
