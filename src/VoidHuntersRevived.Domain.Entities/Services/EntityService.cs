using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Common;
using Serilog;
using VoidHuntersRevived.Common.Entities;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common.Simulations.Engines;
using Guppy.Common.Collections;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Engines;
using Guppy.Common.Attributes;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    [Sequence<InitializeSequence>(InitializeSequence.PreInitialize)]
    internal partial class EntityService : BasicEngine, IEntityService, IQueryingEntitiesEngine
    {
        private readonly IEngineService _engines;
        private readonly ILogger _logger;
        private readonly EntityTypeService _types;
        private readonly SimpleEntitiesSubmissionScheduler _scheduler;

        public EntityService(
            IEngineService engines, 
            ILogger logger, 
            EntityTypeService types,
            SimpleEntitiesSubmissionScheduler scheduler,
            IEnumerable<VoidHuntersEntityDescriptor> descriptors)
        {
            _engines = engines;
            _logger = logger;
            _types = types;
            _scheduler = scheduler;
            _descriptors = new DoubleDictionary<Id<VoidHuntersEntityDescriptor>, Type, IVoidHuntersEntityDescriptorEngine>();
        }

        public EntitiesDB entitiesDB { get; set; } = null!;

        public override void Ready()
        {
            base.Ready();

            foreach (VoidHuntersEntityDescriptorEngine engine in _engines.OfType<IVoidHuntersEntityDescriptorEngine>())
            {
                _descriptors.TryAdd(engine.Descriptor.Id, engine.Descriptor.GetType(), engine);
            }
        }
    }
}
