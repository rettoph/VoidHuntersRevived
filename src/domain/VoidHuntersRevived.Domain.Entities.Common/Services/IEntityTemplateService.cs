using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTemplateService
    {
        IEntityTemplate GetByKey(Key<IEntityTemplate> key);

        IEnumerable<IEntityTemplate> WithComponent<TComponent>()
            where TComponent : unmanaged, IEntityComponent;
    }
}
