using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Common.Components;

namespace VoidHuntersRevived.Game.Entities.Loaders
{
    [AutoLoad]
    public sealed class EntityLoader : IEntityTypeLoader
    {
        public void Configure(IEntityTypeService entities)
        {
            entities.Configure(EntityTypes.Ship, configuration =>
            {
                configuration.Has<Helm>()
                    .Has<Body>();
            });

            entities.Configure(EntityTypes.UserShip, configuration =>
            {
                configuration.Inherits(EntityTypes.Ship)
                    .Has<UserOwned>();
            });
        }
    }
}
