using Guppy.Tests.Common;
using Guppy.Tests.Common.Mocks;
using VoidHuntersRevived.Domain.Entities.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Mockers
{
    public class EntitySpawnServiceMocker : MockBuilder<EntitySpawnService>
    {
        public Mocker<IStepEventService> StepEventServiceMocker { get; } = new Mocker<IStepEventService>();
        public EntityQueryServiceMocker EntityQueryServiceMocker { get; } = new EntityQueryServiceMocker();
        public EntitySpawnService EntitySpawnService => this.GetInstance();

        protected override EntitySpawnService Build()
        {
            return new EntitySpawnService(
                eventService: this.StepEventServiceMocker.Object,
                this.EntityQueryServiceMocker.EntityQueryService);
        }
    }
}
