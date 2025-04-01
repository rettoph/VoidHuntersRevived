using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Pieces.Services;
using VoidHuntersRevived.Tests.Common.Entities.Builders;

namespace VoidHuntersRevived.Tests.Common.Ships.Builders
{
    public class TreeServiceBuilder : Builder<TreeService>
    {
        public required EntityQueryServiceBuilder EntityQueryServiceBuilder { get; init; }
        public required EntitySpawnServiceBuilder EntitySpawnServiceBuilder { get; init; }

        protected override TreeService Build()
        {
            return new TreeService(
                entityQueryService: this.EntityQueryServiceBuilder.Object,
                entitySpawnService: this.EntitySpawnServiceBuilder.Object);
        }
    }
}
