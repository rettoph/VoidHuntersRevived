using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Factories;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTemplateFactoryService
    {
        IEntityTemplateFactory GetByKey(Key<IEntityTemplate> key);

        IEntityTemplateFactory GetByType(IEntityTemplate entityTemplate);
    }
}
