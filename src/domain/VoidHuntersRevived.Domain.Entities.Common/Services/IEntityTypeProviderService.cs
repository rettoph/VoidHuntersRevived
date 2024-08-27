using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Providers;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTypeProviderService
    {
        IEntityTypeProvider GetByKey(Key<IEntityType> key);

        IEntityTypeProvider GetByType(IEntityType entityType);
    }
}
