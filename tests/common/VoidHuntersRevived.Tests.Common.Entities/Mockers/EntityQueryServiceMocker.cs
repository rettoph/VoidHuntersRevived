using Guppy.Core.Logging.Common;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Mocks;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Domain.Entities.Extensions.Svelto;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Mockers
{
    public class EntityQueryServiceMocker : BaseMockerBuilder<EntityQueryService>
    {
        public EnginesRoot EnginesRoot { get; set; } = new EnginesRoot(new EntitiesSubmissionScheduler());
        public EntitiesDB EntitiesDB => this.EnginesRoot.GetEntitiesDB();
        public Mocker<ILogger> LoggerMocker { get; set; } = new Mocker<ILogger>();

        public EntityQueryService EntityQueryService => this.GetInstance();

        protected override EntityQueryService Build()
        {
            return new EntityQueryService(
                entitiesDb: this.EntitiesDB,
                logger: this.LoggerMocker.GetInstance());
        }
    }
}
