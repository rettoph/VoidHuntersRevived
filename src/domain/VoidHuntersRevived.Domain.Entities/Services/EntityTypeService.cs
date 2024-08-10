using Guppy.Core.Common;
using Guppy.Core.Resources.Common.Services;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityTypeService : IEntityTypeService
    {
        private readonly Dictionary<IKey<IEntityType>, IEntityType> _entityTypes;

        public EntityTypeService(
            IFiltered<IEntityType> entityTypes,
            IResourceService resources)
        {
            _entityTypes = entityTypes
                .Concat(resources.GetValues<IEntityType>().Select(x => x.Value))
                .DistinctBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x);
        }

        public IEnumerable<IEntityType> GetAll()
        {
            return _entityTypes.Values;
        }

        public IEntityType GetByKey(IKey<IEntityType> key)
        {
            return _entityTypes[key];
        }
    }
}
