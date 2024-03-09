using VoidHuntersRevived.Common.Entities.Initializers;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityTypeInitializerService
    {
        IEnumerable<IEntityTypeInitializer> GetAll();
        IEntityTypeInitializer Get(IEntityType type);
    }
}
