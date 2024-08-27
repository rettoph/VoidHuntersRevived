using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Exceptions
{
    public class EntityProviderTypeException : Exception
    {
        public readonly Key<IEntityType> Key;

        public EntityProviderTypeException(Key<IEntityType> key, string? message) : base(message)
        {
            this.Key = key;
        }
    }
}
