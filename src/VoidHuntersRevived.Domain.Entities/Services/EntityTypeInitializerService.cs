using Guppy.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Providers;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityTypeInitializerService : IEntityTypeInitializerService
    {
        private Dictionary<IEntityType, IEntityTypeInitializer> _initializers;

        public EntityTypeInitializerService(IFiltered<IEntityInitializer> providers)
        {
            EntityTypeInitializerBuilderService builder = new EntityTypeInitializerBuilderService();
            foreach (IEntityInitializer provider in providers.Instances)
            {
                provider.Initialize(builder);
            }

            _initializers = builder.Build();
        }

        public IEntityTypeInitializer Get(IEntityType type)
        {
            return _initializers[type];
        }

        public IEnumerable<IEntityTypeInitializer> GetAll()
        {
            return _initializers.Values;
        }
    }
}
