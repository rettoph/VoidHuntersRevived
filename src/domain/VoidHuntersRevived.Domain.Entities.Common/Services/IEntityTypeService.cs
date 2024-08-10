using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTypeService
    {
        IEntityType GetByKey(IKey<IEntityType> key);

        IEnumerable<IEntityType> GetAll();
    }
}
