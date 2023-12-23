using FixedMath.NET;
using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Constants;
using VoidHuntersRevived.Common.Ships.Components;

namespace VoidHuntersRevived.Game.Common.Loaders
{
    [AutoLoad]
    public sealed class EntityTypeLoader : IEntityTypeLoader
    {
        public void Configure(IEntityTypeService entityTypes)
        {
            entityTypes.Configure(EntityTypes.UserShip, configuration =>
            {
                configuration.InitializeComponent<Awake>(new Awake(sleepingAllowed: false));
                configuration.InitializeComponent<TractorBeamEmitter>(id => new TractorBeamEmitter(id));
                configuration.InitializeComponent<Collision>(new Collision()
                {
                    Categories = CollisionGroups.ShipCategories,
                    CollidesWith = CollisionGroups.ShipCollidesWith
                });
                configuration.InitializeComponent<PhysicsBubble>(new PhysicsBubble()
                {
                    Enabled = true,
                    Radius = (Fix64)25
                });
            });

            entityTypes.Configure(EntityTypes.Chain, configuration =>
            {
                configuration.InitializeComponent<Collision>(new Collision()
                {
                    Categories = CollisionGroups.FreeFloatingCategories,
                    CollidesWith = CollisionGroups.FreeFloatingCollidesWith
                });
            });
        }
    }
}
