using Guppy.Core.Resources.Common;
using Guppy.Core.Serialization.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    [PolymorphicJsonType<IEntityComponent>(nameof(Team))]
    public struct Team : IEntityComponent
    {
        public readonly Id<Team> Id;
        public Resource<string> Name;

        public Team(Resource<string> name)
        {
            this.Id = Id<Team>.FromString(name.Name);
            this.Name = name;
        }
    }
}
