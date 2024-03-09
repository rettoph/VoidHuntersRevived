using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Constants;

namespace VoidHuntersRevived.Game.Core.Initializers
{
    [AutoLoad]
    internal class ChainInitializer : BaseEntityInitializer
    {
        public ChainInitializer() : base([ EntityTypes.Chain ])
        {
            this.WithInstanceInitializer(EntityTypes.Chain, this.InitializeChain);
        }

        private void InitializeChain(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init(new Collision()
            {
                Categories = CollisionGroups.FreeFloatingCategories,
                CollidesWith = CollisionGroups.FreeFloatingCollidesWith
            });
        }
    }
}
