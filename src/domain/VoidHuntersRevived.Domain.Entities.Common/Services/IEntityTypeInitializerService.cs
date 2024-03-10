using VoidHuntersRevived.Domain.Entities.Common.Initializers;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTypeInitializerService
    {
        IEnumerable<IEntityTypeInitializer> GetAll();
        IEntityTypeInitializer Get(IEntityType type);
    }
}
