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

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService : BasicEngine, IEntityService, IQueryingEntitiesEngine
    {
        private readonly Lazy<IScopedEntityDescriptorService> _descriptors;
        private readonly ILogger _logger;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly EntityTypeService _types;
        private readonly SimpleEntitiesSubmissionScheduler _scheduler;

        public EntityService(
            Lazy<IScopedEntityDescriptorService> descriptors, 
            ILogger logger, 
            EnginesRoot enginesRoot, 
            EntityTypeService types,
            SimpleEntitiesSubmissionScheduler scheduler)
        {
            _descriptors = descriptors;
            _logger = logger;
            _factory = enginesRoot.GenerateEntityFactory();
            _functions = enginesRoot.GenerateEntityFunctions();
            _types = types;
            _scheduler = scheduler;
        }

        public EntitiesDB entitiesDB { get; set; } = null!;
    }
}
