using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public class EntityTemplateFragment
    {
        public required Key<IEntityTemplate> Key { get; init; }

        public EntityTemplateFlagsEnum Flags { get; init; } = EntityTemplateFlagsEnum.None;

        public Key<IEntityTemplate>? Inherit { get; init; } = null;

        public IEntityComponent[] Components { get; init; } = [];

        public Type[] RequiredComponents { get; init; } = [];
    }
}