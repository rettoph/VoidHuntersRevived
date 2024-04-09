using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityContextService
    {
        EntityContext GetByKey(string key);

        bool TryGetByKey(string key, [MaybeNullWhen(false)] out EntityContext piece);

        EntityContext[] All<TDescriptor>()
            where TDescriptor : VoidHuntersEntityDescriptor;

        EntityContext[] All();
    }
}
