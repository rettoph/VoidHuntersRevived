using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Services
{
    public class EntityServiceBuilder : BaseInstanceBuilder<EntityService>
    {
        public EntityTemplateServiceBuilder EntityTemplateService;
        public Mocker<EntityQueryService> EntityQueryService;
        public Mocker<EntitySpawnService> EntitySpawnService;
        public Mocker<EntitySerializationService> EntitySerializationService;

        public EntityServiceBuilder(EntityTemplateServiceBuilder? entityTemplateServiceBuilder = null)
        {
            this.EntityTemplateService = entityTemplateServiceBuilder ?? new EntityTemplateServiceBuilder();
            this.EntityQueryService = new Mocker<EntityQueryService>();
            this.EntitySpawnService = new Mocker<EntitySpawnService>();
            this.EntitySerializationService = new Mocker<EntitySerializationService>();
        }

        protected override EntityService build()
        {
            return new EntityService(
                this.EntityTemplateService.GetLazy<IEntityTemplateService>(),
                this.EntityQueryService.GetLazy<IEntityQueryService>(),
                this.EntitySpawnService.GetLazy<IEntitySpawnService>(),
                this.EntitySerializationService.GetLazy<IEntitySerializationService>());
        }
    }
}
