using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Exceptions
{
    public class EntityProviderTypeException(Key<IEntityType> key, string? message) : Exception(message)
    {
        public readonly Key<IEntityType> Key = key;
    }
}
