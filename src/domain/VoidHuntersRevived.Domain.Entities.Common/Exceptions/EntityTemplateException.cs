using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Exceptions
{
    public class EntityTemplateException(Key<IEntityTemplate> key, string? message, Exception? innerException = null) : Exception(message, innerException)
    {
        public readonly Key<IEntityTemplate> Key = key;
    }
}
