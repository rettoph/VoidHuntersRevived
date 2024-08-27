using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Exceptions
{
    public class EntityTypeException : Exception
    {
        public readonly Key<IEntityType> Key;

        public EntityTypeException(Key<IEntityType> key, string? message, Exception? innerException = null) : base(message, innerException)
        {
            this.Key = key;
        }
    }
}
