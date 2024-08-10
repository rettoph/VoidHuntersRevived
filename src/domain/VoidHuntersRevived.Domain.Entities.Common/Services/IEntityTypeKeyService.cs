using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTypeKeyService
    {
        IKey<T>[] GetAll<T>()
            where T : IEntityType;

        IKey<IEntityType>[] GetAllImplementing(IKey<IEntityType> key);
    }
}
