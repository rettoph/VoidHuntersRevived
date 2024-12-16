using Guppy.Core.Resources.Common;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    public readonly struct Team : IEntityComponent, IHasMany<TeamMember>
    {
        public readonly Id<Team> Id;
        public readonly ResourceKey<string> Name;
        public EntityFilterId<TeamMember> ChildrenFilterId { get; }
        public TeamMember TeamMemberComponent { get; }
        public Team(ResourceKey<string> name)
        {
            this.Id = Id<Team>.FromString(name.Name);
            this.Name = name;
        }

        public Team(EntityLocalId localId, ResourceKey<string> name)
        {
            this.Id = Id<Team>.FromString(name.Name);
            this.Name = name;
            this.ChildrenFilterId = new EntityFilterId<TeamMember>();
            this.TeamMemberComponent = new TeamMember(localId);
        }
    }
}
