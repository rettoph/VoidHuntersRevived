using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Constants;
using VoidHuntersRevived.Domain.Ships.Common.Components;

namespace VoidHuntersRevived.Game.Core.Initializers
{
    [AutoLoad]
    internal sealed class UserShipInitializer : BaseEntityInitializer
    {
        public UserShipInitializer() : base([])
        {
            this.WithInstanceInitializer(EntityTypes.UserShip, this.InitializeUserShip);
        }

        private void InitializeUserShip(ref EntityInitializer initializer, in EntityId id)
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
