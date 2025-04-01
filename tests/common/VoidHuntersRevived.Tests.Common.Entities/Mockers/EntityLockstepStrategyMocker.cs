using Guppy.Core.Logging.Common;
using Guppy.Core.Logging.Common.Services;
using Guppy.Tests.Common;
using Moq;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Domain.Entities.Extensions.Svelto;
using VoidHuntersRevived.Domain.Entities.Providers;
using VoidHuntersRevived.Domain.Entities.Systems;
using VoidHuntersRevived.Tests.Common.Builders;
using VoidHuntersRevived.Tests.Common.Entities.Builders;
using VoidHuntersRevived.Tests.Common.Entities.Interfaces;
using VoidHuntersRevived.Tests.Common.Simulations.Extensions;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;

namespace VoidHuntersRevived.Tests.Common.Entities.Mockers
{
    public class EntityLockstepStrategyMocker : DefaultLockstepStrategyMocker, IEntityStrategyMocker
    {
        public EntitiesSubmissionScheduler EntitiesSubmissionScheduler { get; }
        public EnginesRoot EnginesRoot { get; }
        public EntityQueryServiceBuilder EntityQueryServiceBuilder { get; }
        public EntitySpawnServiceBuilder EntitySpawnServiceBuilder { get; }
        public EntityTemplateFragmentServiceMocker EntityTemplateFragmentServiceMocker { get; }
        public EntityTemplateServiceBuilder EntityTemplateServiceBuilder { get; }
        public ComponentSerializerServiceBuilder ComponentSerializerServiceBuilder { get; }
        public EntitySerializationServiceBuilder EntitySerializationServiceBuilder { get; }
        public EntityServiceBuilder EntityServiceBuilder { get; }
        public Mocker<ILoggerService> LoggerServiceMocker { get; }

        public EntityLockstepStrategyMocker()
        {
            this.EntitiesSubmissionScheduler = new EntitiesSubmissionScheduler();
            this.EnginesRoot = new EnginesRoot(this.EntitiesSubmissionScheduler);
            this.LoggerServiceMocker = new Mocker<ILoggerService>()
                .SetupReturn(loggers => loggers.GetLogger(It.IsAny<Type>()), () => new Mocker<ILogger>().Object)
                .SetupReturn(loggers => loggers.GetLogger<It.IsAnyType>(), new InvocationFunc(invocation =>
                {
                    Type loggerContext = invocation.Method.ReturnType.GenericTypeArguments[0];
                    return Mocker.GetGenericInstance(typeof(ILogger<>), loggerContext);
                }));
            this.EntityQueryServiceBuilder = new EntityQueryServiceBuilder()
            {
                EnginesRoot = this.EnginesRoot,
                LoggerMocker = this.LoggerMocker
            };
            this.EntitySpawnServiceBuilder = new EntitySpawnServiceBuilder()
            {
                EntityQueryServiceBuilder = this.EntityQueryServiceBuilder,
                StepEventServiceBuilder = this.DefaultLockstepStepEventServiceBuilder
            };
            this.ComponentSerializerServiceBuilder = new ComponentSerializerServiceBuilder()
            {
                ComponentSerializers = []
            };
            this.EntityTemplateFragmentServiceMocker = new EntityTemplateFragmentServiceMocker();
            this.EntityTemplateServiceBuilder = new EntityTemplateServiceBuilder()
            {
                EnginesRoot = this.EnginesRoot,
                ScopedSystemServiceMocker = this.ScopedSystemServiceMocker,
                UniqueNumberProviderBuilder = new UniqueNumberProviderBuilder(),
                EntityTemplateFragmentServiceMocker = this.EntityTemplateFragmentServiceMocker,
                LoggerServiceMocker = this.LoggerServiceMocker,
                ComponentSerializerServiceBuilder = this.ComponentSerializerServiceBuilder
            };
            this.EntitySerializationServiceBuilder = new EntitySerializationServiceBuilder()
            {
                EntityQueryServiceBuilder = this.EntityQueryServiceBuilder,
                EntitySpawnServiceBuilder = this.EntitySpawnServiceBuilder,
                EntityTemplateServiceBuilder = this.EntityTemplateServiceBuilder,
                LoggerMocker = this.LoggerMocker
            };
            this.EntityServiceBuilder = new EntityServiceBuilder()
            {
                EntityTemplateServiceBuilder = this.EntityTemplateServiceBuilder,
                EntityQueryServiceBuilder = this.EntityQueryServiceBuilder,
                EntitySpawnServiceBuilder = this.EntitySpawnServiceBuilder,
                EntitySerializationServiceBuilder = this.EntitySerializationServiceBuilder
            };

            this.SystemFactories.AddRange([
                x => new EngineSystem(
                    strategy: x,
                    enginesRoot: this.EnginesRoot),
                x => new EntitySpawnServiceEventSystem(
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    strategy: x,
                    entityService: this.EntityServiceBuilder.Object,
                    entityTemplateService: this.EntityTemplateServiceBuilder.Object,
                    logger: this.LoggerMocker.Object),
                x => new EntitySubmissionSystem(
                    scheduler: this.EntitiesSubmissionScheduler),
                x => new InitializeEntityServicesSystem(
                    componentSerializerService: this.ComponentSerializerServiceBuilder.Object,
                    entityTemplateService: this.EntityTemplateServiceBuilder.Object)
            ]);
        }

        protected override void PreBuild()
        {
            base.PreBuild();

            BelongsToSystemProvider belongsToSystemProvider = new(
                entityTemplateFragmentService: this.EntityTemplateFragmentServiceMocker.Object,
                entityQueryService: this.EntityQueryServiceBuilder.Object,
                logger: this.LoggerMocker.Object);

            DisposableSystemProvider disposeSystemProvider = new(
                entityTemplateFragmentService: this.EntityTemplateFragmentServiceMocker.Object,
                entitiesDb: this.EnginesRoot.GetEntitiesDB(),
                logger: this.LoggerMocker.Object);

            this.SystemFactories.AddProviderSystems(belongsToSystemProvider, disposeSystemProvider);
        }
    }
}
