using Guppy.Core.Common;
using Guppy.Core.Resources.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Enums;
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
            IEnumerable<IEntityType> imported = Resource<IEntityType>.GetAll().Select(x => x.Value);

            _initializers = initializers.SelectMany(init => init.ExplicitEntityTypes).Concat(imported).Distinct().ToDictionary(
                keySelector: type => type,
                elementSelector: type => (IEntityTypeInitializer)new EntityTypeInitializerWrapper(type, initializers.Where(init => init.ShouldInitialize(type))));
        }

        public IEntityTypeInitializer Get(IEntityType type)
        {
            return _initializers[type];
        }

        public IEnumerable<IEntityTypeInitializer> GetAll(EntityTypeFlags except)
        {
            return _initializers.Values.Where(x => (x.Type.Flags & except) == 0);
        }
    }
}
