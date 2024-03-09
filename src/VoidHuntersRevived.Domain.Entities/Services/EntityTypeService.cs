using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntityTypeService : IEntityTypeService
    {
        private Dictionary<Id<IEntityType>, IEntityType> _types;

        public EntityTypeService(IEntityTypeInitializerService initializers)
        {
            _types = initializers.GetAll().ToDictionary(x => x.Type.Id, x => x.Type);
        }

        public IEntityType GetById(Id<IEntityType> id)
        {
            return _types[id];
        }
    }
}
