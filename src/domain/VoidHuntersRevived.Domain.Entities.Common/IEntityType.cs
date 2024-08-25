using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public interface IEntityType
    {
        IKey<IEntityType> Key { get; }
        EntityTypeFlags Flags { get; }
        IKey<IEntityType>[] Include { get; }

        HashSet<Type> RequiredComponents { get; }
        ComponentBuilderDictionary Components { get; }
    }
}
