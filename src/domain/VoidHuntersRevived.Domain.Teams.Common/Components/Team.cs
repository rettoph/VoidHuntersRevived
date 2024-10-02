using Guppy.Core.Resources.Common;
using Guppy.Core.Serialization.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    [PolymorphicJsonType<IEntityComponent>(nameof(Team))]
    public struct Team(Resource<string> name) : IEntityComponent
    {
        public readonly Id<Team> Id = Id<Team>.FromString(name.Name);
        public Resource<string> Name = name;
    }
}
