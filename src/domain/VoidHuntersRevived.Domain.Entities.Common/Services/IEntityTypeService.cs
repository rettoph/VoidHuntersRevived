using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTypeService
    {
        IEntityType GetByKey(Key<IEntityType> key);

        IEnumerable<IEntityType> GetAll();

        T[] GetAll<T>()
            where T : IEntityType;
    }
}
