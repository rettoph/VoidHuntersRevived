using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Constants;
using VoidHuntersRevived.Common.Ships.Components;

namespace VoidHuntersRevived.Game.Core.Initializers
{
    [AutoLoad]
    internal sealed class UserShipInitializer : BaseEntityInitializer
    {
        public UserShipInitializer() : base([ ])
        {
            this.WithInstanceInitializer(EntityTypes.UserShip, this.InitializeUserShip);
        }

        private void InitializeUserShip(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
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
        }
    }
}
