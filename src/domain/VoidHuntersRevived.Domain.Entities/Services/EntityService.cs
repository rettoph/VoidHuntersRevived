﻿using Guppy.Common.Attributes;
using Guppy.Common.Collections;
using Serilog;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    [Sequence<InitializeSequence>(InitializeSequence.PreInitialize)]
    internal partial class EntityService : BasicEngine, IEntityService, IQueryingEntitiesEngine, IEngineEngine
    {
        private readonly ILogger _logger;
        private readonly IEntityTypeInitializerService _entityTypeInitializer;
        private readonly SimpleEntitiesSubmissionScheduler _scheduler;
        private readonly EntityReader _reader;
        private readonly EntityWriter _writer;

        public EntityService(
            ILogger logger,
            IEntityTypeInitializerService entityTypeInitializer,
            IEntityTypeService entityTypes,
            SimpleEntitiesSubmissionScheduler scheduler)
        {
            _logger = logger;
            _entityTypeInitializer = entityTypeInitializer;
            _scheduler = scheduler;
            _descriptors = new DoubleDictionary<Id<VoidHuntersEntityDescriptor>, Type, IVoidHuntersEntityDescriptorEngine>();
            _writer = new EntityWriter(this, _logger);
            _reader = new EntityReader(entityTypes, this, _logger);
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
