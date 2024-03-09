using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Initializers
{
    internal sealed class EntityTypeEntityInitializerBuilder : BaseEntityInitializerBuilder
    {
        public readonly IEntityType Type;

        internal EntityTypeEntityInitializerBuilder(IEntityType type)
        {
            this.Type = type;
        }
    }
}
