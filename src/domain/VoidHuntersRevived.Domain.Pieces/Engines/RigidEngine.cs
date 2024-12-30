using Guppy.Core.Common.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Extensions.Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    public sealed class RigidEngine : StrategyEngine,
        IOnSpawnEngine<Rigid, Fixture>,
        IOnDespawnEngine<Rigid, Fixture>
    {
        private readonly ISpace _space;
        private readonly IEntityQueryService _entityQueryService;
        private readonly ILogger _logger;

        public RigidEngine(ISpace space, IEntityQueryService entityQueryService, ILogger logger)
        {
            _space = space;
            _entityQueryService = entityQueryService;
            _logger = logger;

            _space.OnBodyEnabled += this.HandleBodyEnabled;
        }

        private void HandleBodyEnabled(IBody body)
        {
            ref Body bodyComponent = ref _entityQueryService.QueryByLocalId<Body>(body.EntityLocalId);
            ref var rigidFixtureFilter = ref _entityQueryService.GetFilter<Fixture, RigidEngine>(bodyComponent.FixtureFilterId.Id);

            foreach (var (indices, group) in rigidFixtureFilter)
            {
                var (localIds, globalIds, rigids, fixtures, _) = _entityQueryService.QueryEntities<EntityLocalId, EntityGlobalId, Rigid, Fixture>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];

                    Entity<Rigid, Fixture> entity = new(
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
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Rigid, Fixture> entity)
        {
            ref var rigidFixtureFilter = ref _entityQueryService.GetFilter<Fixture, RigidEngine>(entity.Second.BodyFilterId.Id);
            rigidFixtureFilter.Add(in entity.LocalId, in entity.Index);

            if (_entityQueryService.TryQueryByEGID<Enabled>(entity.Second.BodyFilterId.Id, out Enabled enabled) == true)
            {
                if (enabled)
                {
                    EntityLocalId bodyLocalId = entity.Second.BodyFilterId.Id.ToEntityLocalId();
                    IBody body = _space.GetBody(bodyLocalId);
                    this.CreateFixtures(body, entity);
                }
            }
            else
            {
                _logger.Warning("Unable to create rigid fixture {RigidLocalId} on body {BodyEGID}.", entity.LocalId, entity.Second.BodyFilterId.Id);
            }
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group05)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Rigid, Fixture> entity)
        {
            ref var rigidFixtureFilter = ref _entityQueryService.GetFilter<Fixture, RigidEngine>(entity.Second.BodyFilterId.Id);
            rigidFixtureFilter.Remove(in entity.LocalId);

            if (_entityQueryService.TryQueryByEGID<Enabled>(entity.Second.BodyFilterId.Id, out Enabled enabled) == true)
            {
                if (enabled)
                {
                    EntityLocalId bodyLocalId = entity.Second.BodyFilterId.Id.ToEntityLocalId();
                    if (_space.TryGetBody(bodyLocalId, out IBody? body) == true)
                    {
                        this.DestroyFixtures(body, entity);
                    }
                    else
                    {
                        _logger.Warning("Unable to destroy rigid fixture {RigidLocalId} on body {BodyLocalId}. Body not found.", entity.LocalId, bodyLocalId);
                    }
                }

            }
            else
            {
                _logger.Warning("Unable to destroy rigid fixture {RigidLocalId} on body {BodyEGID}. Body egid not found.", entity.LocalId, entity.Second.BodyFilterId.Id);
            }
        }

        private void CreateFixtures(IBody body, Entity<Rigid, Fixture> entity)
        {
            for (uint i = 0; i < entity.First.Template.Value.Shapes.Length; i++)
            {
                FixtureId rigidShapeFixtureId = new(i, entity.LocalId);
                _logger.Verbose("Creating fixture for tree {TreeId}; NodeLocalId = {NodeLocalId}, RigidShapeId = {RigidShapeId}", body.EntityLocalId, entity.LocalId, rigidShapeFixtureId);
                body.Create(rigidShapeFixtureId, entity.First.Template.Value.Shapes[i], entity.Second.LocalTransform.ToFixMatrix());
            }
        }

        private void DestroyFixtures(IBody body, Entity<Rigid, Fixture> entity)
        {
            for (uint i = 0; i < entity.First.Template.Value.Shapes.Length; i++)
            {
                FixtureId rigidShapeFixtureId = new(i, entity.LocalId);
                _logger.Verbose("Destroying fixture for tree {TreeId}; NodeLocalId = {NodeLocalId}, RigidShapeId = {RigidShapeId}", body.EntityLocalId, entity.LocalId, rigidShapeFixtureId);
                body.Destroy(rigidShapeFixtureId);
            }
        }
    }
}
