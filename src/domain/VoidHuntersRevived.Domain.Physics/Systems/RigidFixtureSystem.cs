using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
using Guppy.Game.Common.Systems;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Extensions.Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Systems;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using BodyComponent = VoidHuntersRevived.Domain.Physics.Common.Components.Body;
using FixtureComponent = VoidHuntersRevived.Domain.Physics.Common.Components.Fixture;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    public sealed class RigidFixtureSystem : ISceneSystem,
        IOnSpawnSystem<Rigid, FixtureComponent>,
        IOnDespawnSystem<Rigid, FixtureComponent>
    {
        private readonly ISpace _space;
        private readonly IEntityQueryService _entityQueryService;
        private readonly ILogger _logger;

        public RigidFixtureSystem(ISpace space, IEntityQueryService entityQueryService, ILogger logger)
        {
            this._space = space;
            this._entityQueryService = entityQueryService;
            this._logger = logger;

            this._space.OnBodyEnabled += this.HandleBodyEnabled;
        }

        private void HandleBodyEnabled(IBody body)
        {
            ref var rigidFixtureFilter = ref this._entityQueryService.GetCompositeFilter<BodyComponent, FixtureComponent, Rigid>(body.EntityLocalId);

            foreach (var (indices, group) in rigidFixtureFilter)
            {
                var (localIds, globalIds, rigids, fixtures, _) = this._entityQueryService.QueryEntities<EntityLocalId, EntityGlobalId, Rigid, FixtureComponent>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];

                    Entity<Rigid, FixtureComponent> entity = new(
                        index: index,
                        localId: localIds[index],
                        globalId: globalIds[index],
                        first: ref rigids[index],
                        second: ref fixtures[index]);

                    this.CreateFixtures(body, entity);
                }
            }
        }

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group05)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Rigid, FixtureComponent> entity)
        {
            ref var rigidFixtureFilter = ref this._entityQueryService.GetCompositeFilter<BodyComponent, FixtureComponent, Rigid>(entity.Second);
            rigidFixtureFilter.Add(in entity.LocalId, in entity.Index);

            if (this._entityQueryService.TryQueryByEGID<Enabled>(entity.Second.BodyFilterId.EGID, out Enabled enabled) == true)
            {
                if (enabled)
                {
                    EntityLocalId bodyLocalId = entity.Second.BodyFilterId.EGID.ToEntityLocalId();
                    IBody body = this._space.GetBody(bodyLocalId);
                    this.CreateFixtures(body, entity);
                }
            }
            else
            {
                this._logger.Warning("Unable to create rigid fixture {RigidLocalId} on body {BodyEGID}.", entity.LocalId, entity.Second.BodyFilterId.EGID);
            }
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group05)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Rigid, FixtureComponent> entity)
        {
            ref var rigidFixtureFilter = ref this._entityQueryService.GetCompositeFilter<BodyComponent, FixtureComponent, Rigid>(entity.Second);
            rigidFixtureFilter.Remove(in entity.LocalId);

            if (this._entityQueryService.TryQueryByEGID<Enabled>(entity.Second.BodyFilterId.EGID, out Enabled enabled) == true)
            {
                if (enabled)
                {
                    EntityLocalId bodyLocalId = entity.Second.BodyFilterId.EGID.ToEntityLocalId();
                    if (this._space.TryGetBody(bodyLocalId, out IBody? body) == true)
                    {
                        this.DestroyFixtures(body, entity);
                    }
                    else
                    {
                        this._logger.Warning("Unable to destroy rigid fixture {RigidLocalId} on body {BodyLocalId}. Body not found.", entity.LocalId, bodyLocalId);
                    }
                }

            }
            else
            {
                this._logger.Warning("Unable to destroy rigid fixture {RigidLocalId} on body {BodyEGID}. Body egid not found.", entity.LocalId, entity.Second.BodyFilterId.EGID);
            }
        }

        private void CreateFixtures(IBody body, Entity<Rigid, FixtureComponent> entity)
        {
            for (uint i = 0; i < entity.First.Template.Value.Shapes.Length; i++)
            {
                FixtureId rigidShapeFixtureId = new(i, entity.LocalId);
                this._logger.Verbose("Creating fixture for tree {TreeId}; NodeLocalId = {NodeLocalId}, RigidShapeId = {RigidShapeId}", body.EntityLocalId, entity.LocalId, rigidShapeFixtureId);
                body.Create(rigidShapeFixtureId, entity.First.Template.Value.Shapes[i], entity.Second.LocalTransform.ToFixMatrix());
            }
        }

        private void DestroyFixtures(IBody body, Entity<Rigid, FixtureComponent> entity)
        {
            for (uint i = 0; i < entity.First.Template.Value.Shapes.Length; i++)
            {
                FixtureId rigidShapeFixtureId = new(i, entity.LocalId);
                this._logger.Verbose("Destroying fixture for tree {TreeId}; NodeLocalId = {NodeLocalId}, RigidShapeId = {RigidShapeId}", body.EntityLocalId, entity.LocalId, rigidShapeFixtureId);
                body.Destroy(rigidShapeFixtureId);
            }
        }
    }
}