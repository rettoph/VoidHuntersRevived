using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Exceptions
{
    public class EntityProviderTypeException : Exception
    {
        public readonly IKey<IEntityType> Key;

        public EntityProviderTypeException(IKey<IEntityType> key, string? message) : base(message)
        {
            this.Key = key;
        }
    }
}
