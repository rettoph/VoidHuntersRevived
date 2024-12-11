using Guppy.Core.Common.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
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
            if (_entityQueryService.HasAny<Tree>(body.EntityLocalId.Group) == false)
            {
                _logger.Warning("No Tree detected. BodyEntityLocalId = {BodyEntityLocalId}", body.EntityLocalId);
                return;
            }

            ref var filter = ref _entityQueryService.GetFilter<Node>(body.EntityLocalId, Tree.NodeFilterContextId);
            foreach (var (indices, group) in filter)
            {
                if (_entityQueryService.HasAny<Rigid>(group))
                {
                    var (nodes, rigids, _) = _entityQueryService.QueryEntities<Node, Rigid>(group);

                    for (int i = 0; i < indices.count; i++)
                    {
                        uint index = indices[i];
                        Node node = nodes[index];
                        Rigid rigid = rigids[index];

                        _logger.Verbose("EntityLocalId = {EntityLocalId}, BodyEntityLocalId = {BodyEntityLocalId}", body.EntityLocalId, node.Id.VhId);
                        this.CreateFixtures(body, node, rigid);
                    }
                }
            }
        }

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group05)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Rigid> rigid)
        {
            Node node = _entityQueryService.QueryByGroupIndex<Node>(rigid.GroupIndex);

            if (_entityQueryService.TryQueryById<Enabled>(node.TreeId, out Enabled enabled) == true)
            {
                if (enabled)
                {
                    IBody body = _space.GetBody(node.TreeId.ToLocalEntityId());
                    this.CreateFixtures(body, node, rigid.Value);
                }
            }
            else
            {
                _logger.Warning("Unable to create fixtures for node {NodeId} on tree {TreeId}.", rigid.LocalId, node.TreeId.VhId);
            }
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group05)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Rigid> rigid)
        {
            Node node = _entityQueryService.QueryByGroupIndex<Node>(rigid.GroupIndex);

            if (_entityQueryService.TryQueryById<Enabled>(node.TreeId, out Enabled enabled) == true)
            {
                if (enabled)
                {
                    if (_space.TryGetBody(node.TreeId.ToLocalEntityId(), out IBody? body) == true)
                    {
                        this.DestroyFixtures(body, node, rigid.Value);
                    }
                    else
                    {
                        _logger.Warning("Unable to destroy fixtures for node {NodeId} on tree {TreeId}. Body not found.", rigid.LocalId, node.TreeId.VhId);
                    }
                }

            }
            else
            {
                _logger.Warning("Unable to destroy fixtures for node {NodeId} on tree {TreeId}. Tree not found.", rigid.LocalId, node.TreeId.VhId);
            }
        }

        private void CreateFixtures(IBody body, Node node, Rigid rigid)
        {
            for (int i = 0; i < rigid.Template.Value.Shapes.Length; i++)
            {
                VhId rigidShapeId = node.Id.VhId.Create(i);
                _logger.Verbose("Creating fixture for tree {TreeId}; NodeId = {NodeId}, RigidShapeId = {RigidShapeId}", body.EntityLocalId, node.Id.VhId, rigidShapeId);
                body.Create(rigidShapeId, node.Id, rigid.Template.Value.Shapes[i], node.LocalLocation.Transformation);
            }
        }

        private void DestroyFixtures(IBody body, Node node, Rigid rigid)
        {
            for (int i = 0; i < rigid.Template.Value.Shapes.Length; i++)
            {
                VhId rigidShapeId = node.Id.VhId.Create(i);
                _logger.Verbose("Destroying fixture for tree {TreeId}; NodeId = {NodeId}, RigidShapeId = {RigidShapeId}", body.EntityLocalId, node.Id.VhId, rigidShapeId);
                body.Destroy(rigidShapeId);
            }
        }
    }
}
