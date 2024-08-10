using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Constants;
using VoidHuntersRevived.Domain.Ships.Common.Descriptors;

namespace VoidHuntersRevived.Game.Core.Initializers
{
    [AutoLoad]
    internal class ChainInitializer : BaseEntityTypeProviderInitializer
    {
        public ChainInitializer() : base()
        {
            this.WithEntityTypeInitializer<ChainEntityType>(this.AddComponentBuilders);
        }

        private void AddComponentBuilders(IEntityTypeProvider provider)
        {
            provider.InstanceEntityComponentBuilders.Set(new Collision()
            {
                Categories = CollisionGroups.FreeFloatingCategories,
                CollidesWith = CollisionGroups.FreeFloatingCollidesWith
            });
        }
    }
}
