using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using Guppy.Tests.Common.Mocks;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Services
{
    public sealed class EntityTypeServiceBuilder : BaseInstanceBuilder<EntityTypeService>
    {
        public readonly Mocker<IUniqueNumberProvider> UniqueNumberProviderService;
        public readonly Mocker<IComponentSerializerService> ComponentSerializerService;
        public readonly MockFiltered<IEntityInitializer> EntityInitializers;
        public readonly Mocker<IResourceService> ResourceService;
        public readonly Mocker<EnginesRoot> EnginesRoot;

        public EntityTypeServiceBuilder()
        {
            this.UniqueNumberProviderService = new Mocker<IUniqueNumberProvider>();
            this.ComponentSerializerService = new Mocker<IComponentSerializerService>();
            this.EntityInitializers = new MockFiltered<IEntityInitializer>();
            this.ResourceService = new Mocker<IResourceService>();
            this.EnginesRoot = new Mocker<EnginesRoot>();
        }

        protected override EntityTypeService build()
        {
            return new EntityTypeService(
                this.EntityInitializers,
                this.UniqueNumberProviderService.GetInstance(),
                this.ResourceService.GetInstance(),
                this.ComponentSerializerService.GetInstance().ToLazy(),
                this.EnginesRoot.GetInstance());
        }
    }
}
