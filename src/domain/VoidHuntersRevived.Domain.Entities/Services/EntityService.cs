using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Collections;
using Guppy.Core.Common.Utilities;
using Serilog;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    [Sequence<EngineSequence>(EngineSequence.Group01)]
    internal partial class EntityService : BasicEngine, IEntityService, IQueryingEntitiesEngine, IEngineEngine, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ILifetimeScope _scope;
        private readonly EntitiesSubmissionScheduler _scheduler;
        private readonly UnmanagedReference<IEntityService> _ref;

        private EntityReader _reader;
        private EntityWriter _writer;
        private IEntityTypeInitializerService _entityTypeInitializer;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public EntityService(
            ILogger logger,
            ILifetimeScope scope,
            EntitiesSubmissionScheduler scheduler)
        {
            _logger = logger;
            _scope = scope;
            _scheduler = scheduler;
            _descriptors = new DoubleDictionary<Id<VoidHuntersEntityDescriptor>, Type, IVoidHuntersEntityDescriptorEngine>();

            _writer = null!;
            _reader = null!;
            _entityTypeInitializer = null!;

            _ref = new UnmanagedReference<IEntityService>(this);
        }

        public void Dispose()
        {
            _ref.Dispose(false);
        }

        public void Initialize(IEngineService engines)
        {
            _writer = new EntityWriter(this, _logger);
            _reader = new EntityReader(_scope.Resolve<IEntityTypeService>(), this, _logger);
            _entityTypeInitializer = _scope.Resolve<IEntityTypeInitializerService>();

            foreach (InstanceEntityDescriptorEngine engine in engines.OfType<IVoidHuntersEntityDescriptorEngine>())
            {
                _descriptors.TryAdd(engine.Descriptor.Id, engine.Descriptor.GetType(), engine);
            }
        }

        public UnmanagedReference<IEntityService> GetReference()
        {
            return _ref;
        }
    }
}
