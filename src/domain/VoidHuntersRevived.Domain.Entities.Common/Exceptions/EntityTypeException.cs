using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Exceptions
{
    public class EntityTypeException : Exception
    {
        public readonly IKey<IEntityType> Key;

        public EntityTypeException(IKey<IEntityType> key, string? message) : base(message)
        {
            this.Key = key;
        }
    }
}
