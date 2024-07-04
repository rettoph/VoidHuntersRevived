using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Utilities;
using Serilog;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    [Sequence<EngineSequence>(EngineSequence.Group01)]
    public partial class EntityService : StrategyEngine, IEntityService, IQueryingEntitiesEngine, IDisposable
    {
        private readonly ILogger _logger;
        private readonly EntitiesSubmissionScheduler _scheduler;
        private readonly UnmanagedReference<IEntityService> _ref;

        private EntityReader _reader;
        private EntityWriter _writer;
        private Lazy<IEntityTypeService> _entityTypeService;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public IEntityTypeService Types => _entityTypeService.Value;

        public EntityService(
            ILogger logger,
            Lazy<IEntityTypeService> entityTypeService,
            EntitiesSubmissionScheduler scheduler)
        {
            _logger = logger;
            _entityTypeService = entityTypeService;
            _scheduler = scheduler;

            _writer = null!;
            _reader = null!;

            _ref = new UnmanagedReference<IEntityService>(this);
        }

        public void Dispose()
        {
            _ref.Dispose(false);
        }

        public override void Initialize(IStrategy simulation)
        {
            base.Initialize(simulation);

            _writer = new EntityWriter(this, _logger);
            _reader = new EntityReader(this.Types, this, _logger);
        }

        public UnmanagedReference<IEntityService> GetReference()
        {
            return _ref;
        }
    }
}
