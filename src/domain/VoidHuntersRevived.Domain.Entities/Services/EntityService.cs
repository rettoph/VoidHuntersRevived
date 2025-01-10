using Guppy.Core.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityService : StrategyEngine, IEntityService, IDisposable
    {
        private bool _disposed = false;
        private readonly UnmanagedReference<IEntityService> _ref;

        private readonly Lazy<IEntityTemplateService> _entityTemplateService;
        private readonly Lazy<IEntityQueryService> _entityQueryService;
        private readonly Lazy<IEntitySpawnService> _entitySpawnService;
        private readonly Lazy<IEntitySerializationService> _entitySerializationService;

        public IEntityTemplateService Templates => this._entityTemplateService.Value;
        public IEntityQueryService Query => this._entityQueryService.Value;
        public IEntitySpawnService Spawn => this._entitySpawnService.Value;
        public IEntitySerializationService Serialization => this._entitySerializationService.Value;

        IEntityTemplateService IEntityService.Templates => this.Templates;

        IEntityQueryService IEntityService.Query => this.Query;

        IEntitySpawnService IEntityService.Spawn => this.Spawn;

        IEntitySerializationService IEntityService.Serialization => this.Serialization;

        public EntityService(
            Lazy<IEntityTemplateService> entityTemplateService,
            Lazy<IEntityQueryService> entityQueryService,
            Lazy<IEntitySpawnService> entitySpawnService,
            Lazy<IEntitySerializationService> entitySerialzationService)
        {
            this._entityTemplateService = entityTemplateService;
            this._entityQueryService = entityQueryService;
            this._entitySpawnService = entitySpawnService;
            this._entitySerializationService = entitySerialzationService;

            this._ref = new UnmanagedReference<IEntityService>(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed == false)
            {
                if (disposing == true)
                {
                    this._ref.Dispose(false);
                }

                this._disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public UnmanagedReference<IEntityService> GetReference() => this._ref;
    }
}