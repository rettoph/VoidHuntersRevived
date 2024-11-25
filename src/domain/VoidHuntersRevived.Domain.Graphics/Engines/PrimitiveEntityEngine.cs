using Guppy.Core.Common.Attributes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common.Extensions;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Graphics.Engines
{
    public sealed class PrimitiveEntityEngine<TVertex>(
        IPrimitiveService<TVertex> primitiveService,
        IEntityQueryService entityQueryService
    ) : StrategyEngine, IReactOnAddEx<Common.Components.Primitive<TVertex>>, IOnDrawEngine, IQueryingEntitiesEngine
        where TVertex : unmanaged, IVertexType, IEntityComponent
    {
        private readonly IPrimitiveService<TVertex> _primitiveService = primitiveService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        public EntitiesDB entitiesDB { get; set; } = null!;

        [SequenceGroup<OnDrawSequenceGroup>(OnDrawSequenceGroup.PreDraw)]
        public void OnDraw(GameTime gameTime)
        {
            foreach (IPrimitive<TVertex> primitive in _primitiveService.GetAll())
            {
                this.CopyEntityDataToVertexBuffer(primitive);
            }
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Common.Components.Primitive<TVertex>> entities, ExclusiveGroupStruct groupID)
        {
            var (primitives, nativeIds, _) = entities;
            var (teamMembers, _, count) = this.entitiesDB.QueryEntities<TeamMember>(groupID);

            if (count == 0)
            {
                // Non team-entity. Do not attempt to copy sequence group from team
                for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
                {
                    Common.Components.Primitive<TVertex> primitive = primitives[i];

                    _primitiveService.GetPrimitiveByTypeAndSequenceGroup(primitive.Type, primitive.SequenceGroup)
                        .GetFilter<TVertex>(this.entitiesDB)
                        .Add(nativeIds[i], groupID, i);
                }

                return;
            }

            // Team entity. Check the team to see if it has a maching primitive sequence group
            for (uint i = rangeOfEntities.start; i < rangeOfEntities.end; i++)
            {
                ref Common.Components.Primitive<TVertex> primitive = ref primitives[i];

                ref TeamMember teamMember = ref teamMembers[i];
                if (_entityQueryService.TryQueryByVhId<PrimitiveSequenceGroup<TVertex>>(teamMember.ParentFilterId.VhId, out PrimitiveSequenceGroup<TVertex> teamSequenceGroup) && teamSequenceGroup.IsDefault() == false)
                {
                    primitive = new Common.Components.Primitive<TVertex>(primitive.Type, teamSequenceGroup.Value);
                }

                _primitiveService.GetPrimitiveByTypeAndSequenceGroup(primitive.Type, primitive.SequenceGroup)
                    .GetFilter<TVertex>(this.entitiesDB)
                    .Add(nativeIds[i], groupID, i);
            }
        }

        private void CopyEntityDataToVertexBuffer(IPrimitive<TVertex> primitive)
        {
            ref EntityFilterCollection filter = ref primitive.GetFilter<TVertex>(this.entitiesDB);

            foreach (var (indices, group) in filter)
            {
                primitive.EnsureFit(indices.count);

                var (vertices, statuses, _) = this.entitiesDB.QueryEntities<TVertex, EntityStatus>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    if (statuses[index].IsSpawned)
                    { // Only render entities that have been spawned
                        primitive.SetNextVertexUnsafe(vertices[index]);
                    }
                }
            }
        }
    }
}
