using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityTypeService
    {
        IEntityType GetById(Id<IEntityType> id);

        bool TryGetByKey(string key, [MaybeNullWhen(false)] out IEntityType type);

        IEnumerable<IEntityType> GetAll();

        IEntityType<TDescriptor>[] GetAll<TDescriptor>()
            where TDescriptor : VoidHuntersEntityDescriptor;
    }
}
