using Guppy.Core.Common.Utilities;
using Svelto.ECS.Schedulers;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityService
    {
        IEntityTypeProviderService TypeProviders { get; }
        IEntityQueryService Query { get; }
        IEntitySpawnService Spawn { get; }
        IEntitySerializationService Serialization { get; }
        EntitiesSubmissionScheduler SubmissionScheduler { get; }

        UnmanagedReference<IEntityService> GetReference();
    }
}
