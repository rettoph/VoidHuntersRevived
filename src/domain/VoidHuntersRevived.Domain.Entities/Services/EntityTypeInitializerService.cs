using Guppy.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Initializers;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityTypeInitializerService : IEntityTypeInitializerService
    {
        private Dictionary<IEntityType, IEntityTypeInitializer> _initializers;

        public EntityTypeInitializerService(IFiltered<IEntityInitializer> initializers)
        {
            _initializers = initializers.Instances.SelectMany(init => init.ExplicitEntityTypes).Distinct().ToDictionary(
                keySelector: type => type,
                elementSelector: type => (IEntityTypeInitializer)new EntityTypeInitializer(type, initializers.Instances.Where(init => init.ShouldInitialize(type))));
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
