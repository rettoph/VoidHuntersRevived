using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Pieces.Services;
using VoidHuntersRevived.Tests.Common.Entities.Builders;

namespace VoidHuntersRevived.Tests.Common.Ships.Builders
{
    public class NodeServiceBuilder : Builder<NodeService>
    {
        public required EntityQueryServiceBuilder EntityQueryServiceBuilder { get; init; }

        protected override NodeService Build()
        {
            return new NodeService(
                entityQueryService: this.EntityQueryServiceBuilder.Object);
        }
    }
}
