using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Services
{
    public sealed class EntityTypeProviderServiceBuilder(EntityServiceBuilder? entityService = null) : BaseInstanceBuilder<EntityTemplateFactoryService>
    {
        public Mocker<IUniqueNumberProvider> UniqueNumberProviderService = new Mocker<IUniqueNumberProvider>();
        public readonly EntityServiceBuilder EntityService = entityService ?? new EntityServiceBuilder();
        public Mocker<IComponentSerializerService> ComponentSerializerService = new Mocker<IComponentSerializerService>();
        public Mocker<EnginesRoot> EnginesRoot = new Mocker<EnginesRoot>();

        protected override EntityTemplateFactoryService build()
        {
            return new EntityTemplateFactoryService(
                this.UniqueNumberProviderService.GetInstance(),
                this.EntityService.EntityTemplateService.GetInstance(),
                this.ComponentSerializerService.GetInstance().ToLazy(),
                this.EnginesRoot.GetInstance(),
                this.EntityService.GetInstance());
        }
    }
}
