using Guppy.Core.Common.Utilities;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityService : StrategyEngine, IEntityService, IDisposable
    {
        private readonly UnmanagedReference<IEntityService> _ref;

        public EntitiesDB entitiesDB { get; set; } = null!;

        private readonly Lazy<IEntityTemplateService> _entityTemplateService;
        private readonly Lazy<IEntityTemplateFactoryService> _entityTemplateFactoryService;
        private readonly Lazy<IEntityQueryService> _entityQueryService;
        private readonly Lazy<IEntitySpawnService> _entitySpawnService;
        private readonly Lazy<IEntitySerializationService> _entitySerializationService;

        public IEntityTemplateService Templates => _entityTemplateService.Value;
        public IEntityTemplateFactoryService TemplateFactories => _entityTemplateFactoryService.Value;
        public IEntityQueryService Query => _entityQueryService.Value;
        public IEntitySpawnService Spawn => _entitySpawnService.Value;
        public IEntitySerializationService Serialization => _entitySerializationService.Value;

        IEntityTemplateService IEntityService.Templates => this.Templates;

        IEntityQueryService IEntityService.Query => this.Query;

        IEntitySpawnService IEntityService.Spawn => this.Spawn;

        IEntitySerializationService IEntityService.Serialization => this.Serialization;

        public EntityService(
            Lazy<IEntityTemplateService> entityTemplateService,
            Lazy<IEntityTemplateFactoryService> entityTemplateProviderService,
            Lazy<IEntityQueryService> entityQueryService,
            Lazy<IEntitySpawnService> entitySpawnService,
            Lazy<IEntitySerializationService> entitySerialzationService)
        {
            _entityTemplateService = entityTemplateService;
            _entityTemplateFactoryService = entityTemplateProviderService;
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
