using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.Providers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Providers
{
    [AutoLoad]
    internal class PieceEntityInitializerProvider : IEntityInitializerProvider
    {
        private readonly IPieceTypeService _pieces;

        public PieceEntityInitializerProvider(IPieceTypeService pieces)
        {
            _pieces = pieces;
        }

        public void Initialize(IEntityTypeInitializerBuilderService builder)
        {
            foreach (PieceType piece in _pieces.All())
            {
                builder.Configure(piece.EntityType, builder =>
                {
                    builder.InitializeInstanceComponent(piece.Id);

                    foreach (IPieceComponent component in piece.InstanceComponents.Values)
                    {
                        builder.InitializeInstanceComponent(component);
                    }

                    foreach (IPieceComponent component in piece.StaticComponents.Values)
                    {
                        builder.InitializeStaticComponent(component);
                    }
                });
            }
        }
    }
}
