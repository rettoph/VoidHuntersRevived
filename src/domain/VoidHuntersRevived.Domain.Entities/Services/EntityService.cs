using Autofac;
using Guppy.Common.Attributes;
using Guppy.Common.Collections;
using Serilog;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    [Sequence<InitializeSequence>(InitializeSequence.PreInitialize)]
    internal partial class EntityService : BasicEngine, IEntityService, IQueryingEntitiesEngine, IEngineEngine
    {
        private readonly ILogger _logger;
        private readonly ILifetimeScope _scope;
        private readonly SimpleEntitiesSubmissionScheduler _scheduler;

        private EntityReader _reader;
        private EntityWriter _writer;
        private IEntityTypeInitializerService _entityTypeInitializer;

        public EntityService(
            ILogger logger,
            ILifetimeScope scope,
            SimpleEntitiesSubmissionScheduler scheduler)
        {
            _logger = logger;
            _scope = scope;
            _scheduler = scheduler;
            _descriptors = new DoubleDictionary<Id<VoidHuntersEntityDescriptor>, Type, IVoidHuntersEntityDescriptorEngine>();

            _writer = null!;
            _reader = null!;
            _entityTypeInitializer = null!;
        }

        public EntitiesDB entitiesDB { get; set; } = null!;

        public void Initialize(IEngine[] engines)
        {
            _writer = new EntityWriter(this, _logger);
            _reader = new EntityReader(_scope.Resolve<IEntityTypeService>(), this, _logger);
            _entityTypeInitializer = _scope.Resolve<IEntityTypeInitializerService>();

            foreach (VoidHuntersEntityDescriptorEngine engine in engines.OfType<IVoidHuntersEntityDescriptorEngine>())
            {
                _descriptors.TryAdd(engine.Descriptor.Id, engine.Descriptor.GetType(), engine);
            }
        }
    }
}
