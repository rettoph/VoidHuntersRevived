using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Providers;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTemplateProviderService
    {
        IEntityTemplateProvider GetByKey(Key<IEntityTemplate> key);

        IEntityTemplateProvider GetByType(IEntityTemplate entityTemplate);
    }
}
