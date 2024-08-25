using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Constants;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Descriptors;

namespace VoidHuntersRevived.Game.Core.Initializers
{
    [AutoLoad]
    internal sealed class UserShipInitializer : BaseEntityTypeProviderInitializer
    {
        public UserShipInitializer() : base()
        {
            this.WithEntityTypeInitializer<UserShipEntityType>(this.AddRequiredComponentBuilders);
            this.WithEntityInitializer(UserShipEntityType.UserShipEntityTypeKey, this.InitializeUserShip);
        }

        private void AddRequiredComponentBuilders(IEntityTypeProvider provider)
        {
            provider.Components.Set(new Awake(sleepingAllowed: false));

            provider.Components.Set(new Collision()
            {
                Categories = CollisionGroups.ShipCategories,
                CollidesWith = CollisionGroups.ShipCollidesWith
            });

            provider.Components.Set(new PhysicsBubble()
            {
                Enabled = true,
                Radius = (Fix64)25
            });
        }

        private void InitializeUserShip(IEntityService entities, IEntityTypeProvider provider, EntityId id, ref EntityInitializer initializer)
        {
            initializer.Init(new TractorBeamEmitter(id));
        }
    }
}
