using Guppy.Attributes;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Shared;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Pieces.Common.Initializers
{
    [AutoLoad]
    internal sealed class PieceInitializer : BaseEntityInitializer
    {
        private readonly IResourceProvider _resources;
        private readonly IEntityService _entities;

        public PieceInitializer(IResourceProvider resources, IEntityService entities)
        {
            _resources = resources;
            _entities = entities;

            this.WithInstanceInitializer<PieceDescriptor>(this.InitializeColorScheme);
            this.WithStaticInitializer<PieceDescriptor>(this.InitializeResourceColorScheme);
        }

        private void InitializeResourceColorScheme(ref EntityInitializer initializer)
        {
            ref ResourceColorScheme colors = ref initializer.Get<ResourceColorScheme>();

            colors.Primary.Value = _resources.Get(colors.Primary.Resource);
            colors.Secondary.Value = _resources.Get(colors.Secondary.Resource);
        }

        private void InitializeColorScheme(ref EntityInitializer initializer, in EntityId id)
        {
            InstanceEntity instance = initializer.Get<InstanceEntity>();
            ResourceColorScheme defaults = _entities.QueryByGroupIndex<ResourceColorScheme>(instance.StaticEntity);

            initializer.Init<ColorScheme>(new ColorScheme(Color.Red, Color.Green));
        }
    }
}
