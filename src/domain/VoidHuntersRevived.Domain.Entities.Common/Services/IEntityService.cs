using Guppy.Core.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityService
    {
        IEntityTypeService Types { get; }
        IEntityQueryService Query { get; }
        IEntitySpawnService Spawn { get; }
        IEntitySerializationService Serialization { get; }

        UnmanagedReference<IEntityService> GetReference();
    }
}
