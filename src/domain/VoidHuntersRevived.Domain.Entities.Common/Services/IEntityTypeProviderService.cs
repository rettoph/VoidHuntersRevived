using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Providers;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTypeProviderService
    {
        IEntityTypeProvider GetByKey(IKey<IEntityType> key);

        IEntityTypeProvider[] GetAllByKey(IKey<IEntityType> key);
        IEntityTypeProvider[] GetAllByType<T>()
            where T : IEntityType;

        IEnumerable<Type> GetAllDistinctComponentTypes();
    }
}
