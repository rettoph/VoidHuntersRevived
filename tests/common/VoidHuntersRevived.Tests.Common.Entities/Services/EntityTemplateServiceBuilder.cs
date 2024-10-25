using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Services
{
    public sealed class EntityTemplateServiceBuilder : BaseInstanceBuilder<EntityTemplateService>
    {
        public Mocker<IEntityTemplateFragmentService> EntityTemplateFragmentService = new();
        public Mocker<IUniqueNumberProvider> UniqueNumberProviderService = new();
        public Mocker<IComponentSerializerService> ComponentSerializerService = new();
        public Mocker<EnginesRoot> EnginesRoot = new();

        protected override EntityTemplateService build()
        {
            return new EntityTemplateService(
                this.UniqueNumberProviderService.GetInstance(),
                this.EntityTemplateFragmentService.GetInstance(),
                this.ComponentSerializerService.GetInstance().ToLazy(),
                this.EnginesRoot.GetInstance());
        }
    }
}
