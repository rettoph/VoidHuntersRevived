using Guppy.Attributes;
using VoidHuntersRevived.Common.Simulations.Loaders;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Common.Components;

namespace VoidHuntersRevived.Game.Entities.Loaders
{
    [AutoLoad]
    public sealed class EntityLoader : IEntityTypeLoader
    {
        public void Configure(IEntityTypeService entities)
        {
            entities.Configure(EntityTypes.Pilot, configuration =>
            {
                configuration.Has<Pilot>();
            });

            entities.Configure(EntityTypes.Ship, configuration =>
            {
                configuration.Has<Helm>();
            });
        }
    }
}
