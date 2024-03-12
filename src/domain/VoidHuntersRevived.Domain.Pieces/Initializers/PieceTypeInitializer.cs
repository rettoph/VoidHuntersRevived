using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Utilities;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Services;

namespace VoidHuntersRevived.Domain.Pieces.Initializers
{
    [AutoLoad]
    internal class PieceTypeInitializer : BaseEntityInitializer
    {
        public PieceTypeInitializer(IPieceTypeService pieces)
        {
            this.Order = -1;

            foreach (PieceType piece in pieces.All())
            {
                StaticEntityInitializerDelegate? staticInitializer = EntityInitializerHelper.BuildStaticEntityInitializerDelegate(piece.StaticComponents.Values);
                InstanceEntityInitializerDelegate? instanceInitializer = (ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init(piece.Id);
                };

                instanceInitializer += EntityInitializerHelper.BuildInstanceEntityInitializerDelegate(piece.InstanceComponents.Values);

                this.WithStaticInitializer(piece.EntityType, staticInitializer);
                this.WithInstanceInitializer(piece.EntityType, instanceInitializer);
            }
        }
    }
}
