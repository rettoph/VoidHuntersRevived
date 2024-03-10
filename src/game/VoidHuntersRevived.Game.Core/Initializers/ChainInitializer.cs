using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Constants;

namespace VoidHuntersRevived.Game.Core.Initializers
{
    [AutoLoad]
    internal class ChainInitializer : BaseEntityInitializer
    {
        public ChainInitializer() : base()
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
