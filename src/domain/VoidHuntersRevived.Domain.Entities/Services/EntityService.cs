using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Utilities;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    [Sequence<EngineSequence>(EngineSequence.Group01)]
    public partial class EntityService : StrategyEngine, IEntityService, IDisposable
    {
        private readonly UnmanagedReference<IEntityService> _ref;

        public EntitiesDB entitiesDB { get; set; } = null!;


        private readonly Lazy<EntityTypeService> _entityTypeService;
        private readonly Lazy<EntityQueryService> _entityQueryService;
        private readonly Lazy<EntitySpawnService> _entitySpawnService;
        private readonly Lazy<EntitySerializationService> _entitySerializationService;

        public EntityTypeService Types => _entityTypeService.Value;
        public EntityQueryService Query => _entityQueryService.Value;
        public EntitySpawnService Spawn => _entitySpawnService.Value;
        public EntitySerializationService Serialization => _entitySerializationService.Value;

        IEntityTypeService IEntityService.Types => this.Types;

        IEntityQueryService IEntityService.Query => this.Query;

        IEntitySpawnService IEntityService.Spawn => this.Spawn;

        IEntitySerializationService IEntityService.Serialization => this.Serialization;

        public EntityService(
            Lazy<EntityTypeService> entityTypeService,
            Lazy<EntityQueryService> entityQueryService,
            Lazy<EntitySpawnService> entitySpawnService,
            Lazy<EntitySerializationService> entitySerialzationService)
        {
            _entityTypeService = entityTypeService;
            _entityQueryService = entityQueryService;
            _entitySpawnService = entitySpawnService;
            _entitySerializationService = entitySerialzationService;

            _ref = new UnmanagedReference<IEntityService>(this);
        }

        public void Dispose()
        {
            _ref.Dispose(false);
        }

        public UnmanagedReference<IEntityService> GetReference()
        {
            return _ref;
        }
    }
}
