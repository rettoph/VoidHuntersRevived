using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTypeService
    {
        IEntityType GetById(Id<IEntityType> id);

        IEnumerable<IEntityType> GetAll();
    }
}
