using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTemplateFragmentService
    {
        EntityTemplateFragment[] GetByKey(Key<IEntityTemplate> key);

        IReadOnlyDictionary<Key<IEntityTemplate>, EntityTemplateFragment[]> GetAll();

        Type[] GetAllDistinctComponentTypes();
    }
}