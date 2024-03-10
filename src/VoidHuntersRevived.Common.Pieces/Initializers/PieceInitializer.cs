using Guppy.Attributes;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components.Instance;
using VoidHuntersRevived.Common.Pieces.Components.Shared;
using VoidHuntersRevived.Common.Pieces.Descriptors;

namespace VoidHuntersRevived.Common.Pieces.Initializers
{
    [AutoLoad]
    internal sealed class PieceInitializer : BaseEntityInitializer
    {
        private readonly IResourceProvider _resources;

        public PieceInitializer(IResourceProvider resources)
        {
            _resources = resources;

            this.WithInstanceInitializer<PieceDescriptor>(this.InitializeColorScheme);
            this.WithStaticInitializer<PieceDescriptor>(this.InitializeResourceColorScheme);
        }

        private void InitializeResourceColorScheme(ref EntityInitializer initializer)
        {
            ref ResourceColorScheme colors = ref initializer.Get<ResourceColorScheme>();

            colors.Primary.Value = _resources.Get(colors.Primary.Resource);
            colors.Secondary.Value = _resources.Get(colors.Secondary.Resource);
        }

        private void InitializeColorScheme(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init<ColorScheme>(new ColorScheme(Color.Red, Color.Green));
        }
    }
}
