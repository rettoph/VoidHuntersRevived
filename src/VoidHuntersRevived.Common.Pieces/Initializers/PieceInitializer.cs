using Guppy.Attributes;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;

namespace VoidHuntersRevived.Common.Pieces.Initializers
{
    [AutoLoad]
    internal sealed class PieceInitializer : BaseEntityInitializer
    {
        public PieceInitializer()
        {
            this.WithInstanceInitializer<PieceDescriptor>(this.InitializeColorPalette);
        }

        private void InitializeColorPalette(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init<ColorPalette>(new ColorPalette(Color.Red, Color.Green));
        }
    }
}
