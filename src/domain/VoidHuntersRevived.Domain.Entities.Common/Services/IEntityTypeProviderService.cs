using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Providers;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTypeProviderService
    {
        IEntityTypeProvider GetByKey(Key<IEntityType> key);

        IEntityTypeProvider[] GetAllByKey(Key<IEntityType> key);
        IEntityTypeProvider[] GetAllByType<T>()
            where T : IEntityType;

        IEnumerable<Type> GetAllDistinctComponentTypes();
    }
}
