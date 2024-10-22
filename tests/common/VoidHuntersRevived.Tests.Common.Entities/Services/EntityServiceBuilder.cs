using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Services
{
    public class EntityServiceBuilder : BaseInstanceBuilder<EntityService>
    {
        public Mocker<IEntityTemplateService> EntityTemplateService;
        public EntityTypeProviderServiceBuilder EntityTypeProviderService;
        public Mocker<EntityQueryService> EntityQueryService;
        public Mocker<EntitySpawnService> EntitySpawnService;
        public Mocker<EntitySerializationService> EntitySerializationService;

        public EntityServiceBuilder(EntityTypeProviderServiceBuilder? entityTypeProviderService = null)
        {
            this.EntityTemplateService = new Mocker<IEntityTemplateService>();
            this.EntityTypeProviderService = entityTypeProviderService ?? new EntityTypeProviderServiceBuilder(this);
            this.EntityQueryService = new Mocker<EntityQueryService>();
            this.EntitySpawnService = new Mocker<EntitySpawnService>();
            this.EntitySerializationService = new Mocker<EntitySerializationService>();
        }

        protected override EntityService build()
        {
            return new EntityService(
                this.EntityTemplateService.GetLazy(),
                this.EntityTypeProviderService.GetLazy<IEntityTemplateProviderService>(),
                this.EntityQueryService.GetLazy<IEntityQueryService>(),
                this.EntitySpawnService.GetLazy<IEntitySpawnService>(),
                this.EntitySerializationService.GetLazy<IEntitySerializationService>());
        }
    }
}
