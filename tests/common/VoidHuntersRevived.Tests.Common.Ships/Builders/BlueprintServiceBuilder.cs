using Guppy.Core.Assets.Common.Services;
using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Services;

namespace VoidHuntersRevived.Tests.Common.Ships.Builders
{
    public class BlueprintServiceBuilder : Builder<BlueprintService>
    {
        public required List<Blueprint> Blueprints { get; init; }
        public required Mocker<IAssetService> AssetServiceMocker { get; init; }

        protected override BlueprintService Build()
        {
            return new BlueprintService(
                blueprints: this.Blueprints,
                resources: this.AssetServiceMocker.Object);
        }
    }
}
