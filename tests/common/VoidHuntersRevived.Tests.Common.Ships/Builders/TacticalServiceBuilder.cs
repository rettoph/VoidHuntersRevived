using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Ships.Services;
using VoidHuntersRevived.Tests.Common.Entities.Builders;

namespace VoidHuntersRevived.Tests.Common.Ships.Builders
{
    public class TacticalServiceBuilder : Builder<TacticalService>
    {
        public required EntityQueryServiceBuilder EntityQueryServiceBuilder { get; init; }
        protected override TacticalService Build()
        {
            return new TacticalService(
                entityQueryService: this.EntityQueryServiceBuilder.Object);
        }
    }
}
