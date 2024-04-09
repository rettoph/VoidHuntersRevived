using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTypeService
    {
        IEntityType GetById(Id<IEntityType> id);

        IEnumerable<IEntityType> GetAll();

        IEntityType<T>[] GetAll<T>()
            where T : VoidHuntersEntityDescriptor;
    }
}
