using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Entities.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Builders
{
    public class EntitySpawnServiceBuilder : Builder<EntitySpawnService>
    {
        public required IBuilder<IStepEventService> StepEventServiceBuilder { get; init; }
        public required EntityQueryServiceBuilder EntityQueryServiceBuilder { get; init; }

        protected override EntitySpawnService Build()
        {
            return new EntitySpawnService(
                eventService: this.StepEventServiceBuilder.Object,
                entityQueryService: this.EntityQueryServiceBuilder.Object);
        }
    }
}
