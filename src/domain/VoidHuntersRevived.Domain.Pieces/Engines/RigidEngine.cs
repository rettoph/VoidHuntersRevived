using Guppy.Core.Common.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    public sealed class RigidEngine : StrategyEngine,
        IOnSpawnEngine<Rigid>,
        IOnDespawnEngine<Rigid>
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
            if (_entityQueryService.Has<Tree>(body.EntityLocalId.Group) == false)
            {
                _logger.Warning("No Tree detected. BodyEntityLocalId = {BodyEntityLocalId}", body.EntityLocalId);
                return;
            }

            ref var filter = ref _entityQueryService.GetFilter<Node>(body.EntityLocalId, Tree.NodeFilterContextId);
            foreach (var (indices, group) in filter)
            {
                if (_entityQueryService.Has<Rigid>(group))
                {
                    var (nodes, rigids, _) = _entityQueryService.QueryEntities<Node, Rigid>(group);

                    for (int i = 0; i < indices.count; i++)
                    {
                        uint index = indices[i];
                        Node node = nodes[index];
                        Rigid rigid = rigids[index];

                        this.CreateFixtures(body, node, rigid);
                    }
                }
            }
        }

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group05)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Rigid> rigid)
        {
            Node node = _entityQueryService.QueryByGroupIndex<Node>(rigid.GroupIndex);

            if (_entityQueryService.TryQueryByLocalId<Enabled>(node.TreeLocalId, out Enabled enabled) == true)
            {
                if (enabled)
                {
                    IBody body = _space.GetBody(node.TreeLocalId);
                    this.CreateFixtures(body, node, rigid.Component);
                }
            }
            else
            {
                _logger.Warning("Unable to create fixtures for node {NodeLocalId} on tree {TreeLocalId}.", rigid.LocalId, node.TreeLocalId);
            }
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group05)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Rigid> rigid)
        {
            Node node = _entityQueryService.QueryByGroupIndex<Node>(rigid.GroupIndex);

            if (_entityQueryService.TryQueryByLocalId<Enabled>(node.TreeLocalId, out Enabled enabled) == true)
            {
                if (enabled)
                {
                    if (_space.TryGetBody(node.TreeLocalId, out IBody? body) == true)
                    {
                        this.DestroyFixtures(body, node, rigid.Component);
                    }
                    else
                    {
                        _logger.Warning("Unable to destroy fixtures for node {NodeLocalId} on tree {TreeLocalId}. Body not found.", rigid.LocalId, node.TreeLocalId);
                    }
                }

            }
            else
            {
                _logger.Warning("Unable to destroy fixtures for node {NodeLocalId} on tree {TreeLocalId}. Tree not found.", rigid.LocalId, node.TreeLocalId);
            }
        }

        private void CreateFixtures(IBody body, Node node, Rigid rigid)
        {
            for (uint i = 0; i < rigid.Template.Value.Shapes.Length; i++)
            {
                FixtureId rigidShapeFixtureId = new(i, node.LocalId);
                _logger.Verbose("Creating fixture for tree {TreeId}; NodeLocalId = {NodeLocalId}, RigidShapeId = {RigidShapeId}", body.EntityLocalId, node.LocalId, rigidShapeFixtureId);
                body.Create(rigidShapeFixtureId, rigid.Template.Value.Shapes[i], node.LocalLocation.Transformation);
            }
        }

        private void DestroyFixtures(IBody body, Node node, Rigid rigid)
        {
            for (uint i = 0; i < rigid.Template.Value.Shapes.Length; i++)
            {
                FixtureId rigidShapeFixtureId = new(i, node.LocalId);
                _logger.Verbose("Destroying fixture for tree {TreeId}; NodeLocalId = {NodeLocalId}, RigidShapeId = {RigidShapeId}", body.EntityLocalId, node.LocalId, rigidShapeFixtureId);
                body.Destroy(rigidShapeFixtureId);
            }
        }
    }
}
