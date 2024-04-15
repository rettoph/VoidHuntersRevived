using Guppy.Core.Resources;
using Guppy.Core.Resources.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;

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
