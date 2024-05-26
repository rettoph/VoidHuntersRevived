using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTypeInitializerService
    {
        IEnumerable<IEntityTypeInitializer> GetAll(EntityTypeFlags except);
        IEntityTypeInitializer Get(IEntityType type);
    }
}
