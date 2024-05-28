using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Providers;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTypeService
    {
        IEntityType GetById(Id<IEntityType> id);

        bool TryGetByKey(string key, [MaybeNullWhen(false)] out IEntityType type);

        IEnumerable<IEntityType> GetAll();

        IEntityType<TDescriptor>[] GetAll<TDescriptor>()
            where TDescriptor : VoidHuntersEntityDescriptor;

        IEntityTypeProvider GetProviderByType(IEntityType type);
        IEntityTypeProvider GetProviderByTypeId(Id<IEntityType> id);
    }
}
