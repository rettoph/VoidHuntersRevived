using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Providers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Constants;
using VoidHuntersRevived.Common.Ships.Components;

namespace VoidHuntersRevived.Game.Core.Providers
{
    [AutoLoad]
    internal sealed class EntityInitializerProvider : IEntityInitializerProvider
    {
        public void Initialize(IEntityTypeInitializerBuilderService builder)
        {
            builder.Configure(EntityTypes.UserShip, configuration =>
            {
                configuration.InitializeInstance((IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init(new Awake(sleepingAllowed: false));
                    initializer.Init(new TractorBeamEmitter(id));
                    initializer.Init(new Collision()
                    {
                        Categories = CollisionGroups.ShipCategories,
                        CollidesWith = CollisionGroups.ShipCollidesWith
                    });
                    initializer.Init(new PhysicsBubble()
                    {
                        Enabled = true,
                        Radius = (Fix64)25
                    });
                });
            });

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
