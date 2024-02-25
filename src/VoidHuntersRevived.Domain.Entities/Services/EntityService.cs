using Guppy.Common.Attributes;
using Guppy.Common.Collections;
using Serilog;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    [Sequence<InitializeSequence>(InitializeSequence.PreInitialize)]
    internal partial class EntityService : BasicEngine, IEntityService, IQueryingEntitiesEngine, IEngineEngine
    {
        private readonly ILogger _logger;
        private readonly EntityTypeService _types;
        private readonly SimpleEntitiesSubmissionScheduler _scheduler;
        private readonly EntityReader _reader;
        private readonly EntityWriter _writer;

        public EntityService(
            ILogger logger,
            EntityTypeService types,
            SimpleEntitiesSubmissionScheduler scheduler)
        {
            _logger = logger;
            _types = types;
            _scheduler = scheduler;
            _descriptors = new DoubleDictionary<Id<VoidHuntersEntityDescriptor>, Type, IVoidHuntersEntityDescriptorEngine>();
            _writer = new EntityWriter(this, _logger);
            _reader = new EntityReader(_types, this, _logger);
        }

        public EntitiesDB entitiesDB { get; set; } = null!;

        public void Initialize(IEngine[] engines)
        {
            foreach (VoidHuntersEntityDescriptorEngine engine in engines.OfType<IVoidHuntersEntityDescriptorEngine>())
            {
                _descriptors.TryAdd(engine.Descriptor.Id, engine.Descriptor.GetType(), engine);
            }
        }
    }
}
