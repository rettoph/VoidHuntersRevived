using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Exceptions
{
    public class EntityTypeException(Key<IEntityType> key, string? message, Exception? innerException = null) : Exception(message, innerException)
    {
        public readonly Key<IEntityType> Key = key;
    }
}
