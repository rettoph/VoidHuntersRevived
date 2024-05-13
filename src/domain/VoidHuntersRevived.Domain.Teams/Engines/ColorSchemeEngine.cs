using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Teams.Engines
{
    [AutoLoad]
    internal class ColorSchemeEngine : BasicEngine, IReactOnAddEx<Instance<TeamMemberEntityDescriptor>>
    {
        private readonly IEntityService _entities;

        public ColorSchemeEngine(IEntityService entities)
        {
            _entities = entities;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Instance<TeamMemberEntityDescriptor>> entities, ExclusiveGroupStruct groupID)
        {
            var (instanceDatas, colorSchemes, teamIds, _) = _entities.QueryEntities<InstanceData, ColorScheme, Id<Team>>(groupID);

            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                ref InstanceData instanceData = ref instanceDatas[i];
                ref ColorScheme instanceColorScheme = ref colorSchemes[i];
                ref Id<Team> teamId = ref teamIds[i];


            }
        }
    }
}
