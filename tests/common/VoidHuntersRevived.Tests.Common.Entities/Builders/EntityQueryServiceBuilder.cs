using Guppy.Core.Logging.Common;
using Guppy.Tests.Common;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Extensions.Svelto;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Builders
{
    public class EntityQueryServiceBuilder : Builder<EntityQueryService>
    {
        public required EnginesRoot EnginesRoot { get; init; }
        public required Mocker<ILogger> LoggerMocker { get; init; }

        protected override EntityQueryService Build()
        {
            return new EntityQueryService(
                entitiesDb: this.EnginesRoot.GetEntitiesDB(),
                logger: this.LoggerMocker.Object);
        }
    }
}
