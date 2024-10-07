using Guppy.Core.Common.Attributes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Graphics.Engines
{
    [StrategyFilter(StrategyTypeEnum.Predictive)]
    public sealed class PrimitiveEntityEngine<TVertex>(
        IPrimitiveService<TVertex> vertexTypeService,
        IEntityQueryService entityQueryService
    ) : StrategyEngine, IReactOnAddEx<Common.Components.Primitive<TVertex>>, IOnDrawEngine, IQueryingEntitiesEngine
        where TVertex : unmanaged, IVertexType, IEntityComponent
    {
        private readonly IPrimitiveService<TVertex> _vertexTypeService = vertexTypeService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        public EntitiesDB entitiesDB { get; set; } = null!;

        [SequenceGroup<OnDrawSequenceGroup>(OnDrawSequenceGroup.PreDraw)]
        public void OnDraw(GameTime gameTime)
        {
            // throw new NotImplementedException();

            // foreach (IVertexBuffer<TVertex> vertexBuffer in _vertexBuffers)
            // {
            //     this.CopySpawnedEntityVertexData(vertexBuffer);
            // }
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Common.Components.Primitive<TVertex>> entities, ExclusiveGroupStruct groupID)
        {
            var (primitives, nativeIds, _) = entities;
            var (belongsToTeams, _, count) = this.entitiesDB.QueryEntities<BelongsTo<Team, TeamMember>>(groupID);

            if (count == 0)
            {
                // Non team-entity. Do not attempt to copy sequence group from team
                for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
                {
                    Common.Components.Primitive<TVertex> primitive = primitives[i];

                    _vertexTypeService.GetPrimitiveByTypeAndSequenceGroup(primitive.Type, primitive.SequenceGroup)
                        .GetFilter<TVertex>(this.entitiesDB)
                        .Add(nativeIds[i], groupID, i);
                }

                return;
            }

            // Team entity. Check the team to see if it has a maching primitive sequence group
            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                ref Common.Components.Primitive<TVertex> primitive = ref primitives[i];

                ref BelongsTo<Team, TeamMember> belongsToTeam = ref belongsToTeams[i];
                if (_entityQueryService.TryQueryByVhId<PrimitiveSequenceGroup<TVertex>>(belongsToTeam.OwnerVhId, out PrimitiveSequenceGroup<TVertex> teamSequenceGroup) && teamSequenceGroup.IsDefault() == false)
                {
                    primitive = new Common.Components.Primitive<TVertex>(primitive.Type, teamSequenceGroup.Value);
                }

                _vertexTypeService.GetPrimitiveByTypeAndSequenceGroup(primitive.Type, primitive.SequenceGroup)
                    .GetFilter<TVertex>(this.entitiesDB)
                    .Add(nativeIds[i], groupID, i);
            }
        }
    }
}
