using Guppy.Core.Assets.Common;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    public readonly struct Team : IEntityComponent, IHasMany<TeamMember>
    {
        public readonly Id<Team> Id;
        public readonly AssetKey<string> Name;
        public EntityFilterId<TeamMember> ChildrenFilterId { get; }
        public TeamMember TeamMemberComponent { get; }
        public Team(AssetKey<string> name)
        {
            this.Id = Id<Team>.FromString(name.Name);
            this.Name = name;
        }

        public Team(EntityLocalId localId, AssetKey<string> name)
        {
            this.Id = Id<Team>.FromString(name.Name);
            this.Name = name;
            this.ChildrenFilterId = EntityFilterId<TeamMember>.Create<Team>(localId);
            this.TeamMemberComponent = new TeamMember(localId);
        }
    }
}