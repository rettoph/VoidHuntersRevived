using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityResourceService<T>
        where T : IEntityResource<T>
    {
        T GetById(Id<T> id);

        IEnumerable<T> GetAll();
    }
}
