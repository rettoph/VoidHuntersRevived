using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Services
{
    public sealed class EntityTypeProviderServiceBuilder : BaseInstanceBuilder<EntityTypeProviderService>
    {
        public Mocker<IUniqueNumberProvider> UniqueNumberProviderService;
        public readonly EntityServiceBuilder EntityService;
        public Mocker<IComponentSerializerService> ComponentSerializerService;
        public Mocker<EnginesRoot> EnginesRoot;

        public EntityTypeProviderServiceBuilder(EntityServiceBuilder? entityService = null)
        {
            this.UniqueNumberProviderService = new Mocker<IUniqueNumberProvider>();
            this.EntityService = entityService ?? new EntityServiceBuilder();
            this.ComponentSerializerService = new Mocker<IComponentSerializerService>();
            this.EnginesRoot = new Mocker<EnginesRoot>();
        }

        protected override EntityTypeProviderService build()
        {
            return new EntityTypeProviderService(
                this.UniqueNumberProviderService.GetInstance(),
                this.EntityService.EntityTypeService.GetInstance(),
                this.ComponentSerializerService.GetInstance().ToLazy(),
                this.EnginesRoot.GetInstance(),
                this.EntityService.GetInstance());
        }
    }
}
