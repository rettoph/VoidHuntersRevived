using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Services
{
    public class EntityServiceBuilder : BaseInstanceBuilder<EntityService>
    {
        public readonly Mocker<EntityTypeService> EntityTypeService;
        public readonly Mocker<EntityQueryService> EntityQueryService;
        public readonly Mocker<EntitySpawnService> EntitySpawnService;
        public readonly Mocker<EntitySerializationService> EntitySerializationService;

        public EntityServiceBuilder()
        {
            this.EntityTypeService = new Mocker<EntityTypeService>();
            this.EntityQueryService = new Mocker<EntityQueryService>();
            this.EntitySpawnService = new Mocker<EntitySpawnService>();
            this.EntitySerializationService = new Mocker<EntitySerializationService>();
        }

        protected override EntityService build()
        {
            return new EntityService(
                this.EntityTypeService.GetLazy(),
                this.EntityQueryService.GetLazy(),
                this.EntitySpawnService.GetLazy(),
                this.EntitySerializationService.GetLazy());
        }
    }
}
