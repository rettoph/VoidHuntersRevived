using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTemplateService
    {
        IEntityTemplate GetByKey(Key<IEntityTemplate> key);

        IEnumerable<IEntityTemplate> GetAll();

        T[] GetAll<T>()
            where T : IEntityTemplate;

        Type[] GetAllDistinctComponentTypes();
    }
}
