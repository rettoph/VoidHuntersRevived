using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Providers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Constants;

namespace VoidHuntersRevived.Game.Core.Initializers
{
    [AutoLoad]
    internal class ChainInitializer : IEntityInitializer
    {
        public void Initialize(IEntityTypeInitializerBuilderService builder)
        {
            builder.Configure(EntityTypes.Chain, configuration =>
            {
                configuration.InitializeInstance((IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init(new Collision()
                    {
                        Categories = CollisionGroups.FreeFloatingCategories,
                        CollidesWith = CollisionGroups.FreeFloatingCollidesWith
                    });
                });
            });
        }
    }
}
