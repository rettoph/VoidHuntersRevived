using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using Guppy.Tests.Common.Mocks;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Services
{
    public sealed class EntityTypeServiceBuilder : BaseInstanceBuilder<EntityTypeService>
    {
        public readonly Mocker<IComponentSerializerService> ComponentSerializerService;
        public readonly MockFiltered<IEntityInitializer> EntityInitializers;
        public readonly Mocker<IResourceService> ResourceService;
        public readonly Mocker<EnginesRoot> EnginesRoot;

        public EntityTypeServiceBuilder()
        {
            ComponentSerializerService = new Mocker<IComponentSerializerService>();
            EntityInitializers = new MockFiltered<IEntityInitializer>();
            ResourceService = new Mocker<IResourceService>();
            EnginesRoot = new Mocker<EnginesRoot>();
        }

        protected override EntityTypeService build()
        {
            return new EntityTypeService(
                EntityInitializers,
                ResourceService.GetInstance(),
                ComponentSerializerService.GetInstance().ToLazy(),
                EnginesRoot.GetInstance());
        }
    }
}
