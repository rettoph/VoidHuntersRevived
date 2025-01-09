using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public readonly struct EntityTemplate(Key<IEntityTemplate> key) : IEntityComponent
    {
        public readonly Key<IEntityTemplate> Key = key;
    }
}