using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Exceptions
{
    public class EntityProviderTypeException(Key<IEntityTemplate> key, string? message) : Exception(message)
    {
        public readonly Key<IEntityTemplate> Key = key;
    }
}
