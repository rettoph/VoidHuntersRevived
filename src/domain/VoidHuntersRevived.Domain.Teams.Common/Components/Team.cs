using Guppy.Core.Resources.Common;
using Guppy.Core.Serialization.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    [PolymorphicJsonType<IEntityComponent>(nameof(Team))]
    public readonly struct Team : IEntityComponent, IHasMany<TeamMember>
    {
        public readonly Id<Team> Id;
        public readonly ResourceKey<string> Name;
        public FilterVhId<TeamMember> ChildrenFilterId { get; }
        public TeamMember TeamMemberComponent { get; }
        public Team(ResourceKey<string> name)
        {
            this.Id = Id<Team>.FromString(name.Name);
            this.Name = name;
        }

        public Team(EntityId entityId, ResourceKey<string> name)
        {
            this.Id = Id<Team>.FromString(name.Name);
            this.Name = name;
            this.ChildrenFilterId = new FilterVhId<TeamMember>();
            this.TeamMemberComponent = new TeamMember(entityId);
        }
    }
}
