using Guppy.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Constants;
using VoidHuntersRevived.Common.Ships.Components;

namespace VoidHuntersRevived.Game.Core.Loaders
{
    [AutoLoad]
    internal sealed class EntityTypeLoader : IEntityTypeLoader
    {
        public void Configure(IEntityTypeService entityTypes)
        {
            entityTypes.Configure(EntityTypes.UserShip, configuration =>
            {
                configuration.InitializeInstanceComponent<Awake>(new Awake(sleepingAllowed: false));
                configuration.InitializeInstanceComponent<TractorBeamEmitter>(id => new TractorBeamEmitter(id));
                configuration.InitializeInstanceComponent<Collision>(new Collision()
                {
                    Categories = CollisionGroups.ShipCategories,
                    CollidesWith = CollisionGroups.ShipCollidesWith
                });
                configuration.InitializeInstanceComponent<PhysicsBubble>(new PhysicsBubble()
                {
                    Enabled = true,
                    Radius = (Fix64)25
                });
            });

            entityTypes.Configure(EntityTypes.Chain, configuration =>
            {
                configuration.InitializeInstanceComponent<Collision>(new Collision()
                {
                    Categories = CollisionGroups.FreeFloatingCategories,
                    CollidesWith = CollisionGroups.FreeFloatingCollidesWith
                });
            });
        }
    }
}
