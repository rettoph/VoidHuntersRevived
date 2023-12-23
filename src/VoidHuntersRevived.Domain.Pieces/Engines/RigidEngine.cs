using Guppy.Attributes;
using Guppy.Resources.Providers;
using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal sealed class RigidEngine : BasicEngine,
        IOnSpawnEngine<Rigid>,
        IOnDespawnEngine<Rigid>
    {
        private readonly ISpace _space;
        private readonly IEntityService _entities;
        private readonly ILogger _logger;

        public RigidEngine(ISpace space, IEntityService entities, ILogger logger)
        {
            _space = space;
            _entities = entities;
            _logger = logger;

            _space.OnBodyEnabled += this.HandleBodyEnabled;
        }

        private void HandleBodyEnabled(IBody body)
        {
            if(_entities.HasAny<Tree>(body.Id.EGID.groupID) == false)
            {
                _logger.Warning("{ClassName}::{MethodName} - No Tree detected. BodyId = {BodyId}", nameof(RigidEngine), nameof(HandleBodyEnabled), body.Id.VhId);
                return;
            }

            ref var filter = ref _entities.GetFilter<Node>(body.Id, Tree.NodeFilterContextId);
            foreach (var (indices, group) in filter)
            {
                if(_entities.HasAny<Rigid>(group))
                {
                    var (nodes, rigids, _) = _entities.QueryEntities<Node, Rigid>(group);

                    for (int i = 0; i < indices.count; i++)
                    {
                        uint index = indices[i];
                        Node node = nodes[index];
                        Rigid rigid = rigids[index];

                        _logger.Verbose("{ClassName}::{MethodName} - BodyId = {BodyId}, NodeId = {EntityId}", nameof(RigidEngine), nameof(HandleBodyEnabled), body.Id.VhId, node.Id.VhId);
                        this.CreateFixtures(body, node, rigid);
                    }
                }
            }
        }

        public void OnSpawn(EntityId id, ref Rigid component, in GroupIndex groupIndex)
        {
            _logger.Verbose("{ClassName}::{MethodName} - EntityId = {EntityId}", nameof(RigidEngine), nameof(OnSpawn), id.VhId);

            Node node = _entities.QueryByGroupIndex<Node>(groupIndex);

            if (_entities.TryQueryById<Enabled>(node.TreeId, out Enabled enabled) == true)
            {
                if(enabled)
                {
                    IBody body = _space.GetBody(node.TreeId);
                    this.CreateFixtures(body, node, component);
                }
            }
            else
            {
                _logger.Warning("{ClassName}::{MethodName} - Unable to create fixtures for node {NodeId} on tree {TreeId}.", nameof(RigidEngine), nameof(OnSpawn), id.VhId, node.TreeId.VhId);
            }
        }

        public void OnDespawn(EntityId id, ref Rigid component, in GroupIndex groupIndex)
        {
            _logger.Verbose("{ClassName}::{MethodName} - EntityId = {EntityId}", nameof(RigidEngine), nameof(OnDespawn), id.VhId);

            Node node = _entities.QueryByGroupIndex<Node>(groupIndex);

            if (_entities.TryQueryById<Enabled>(node.TreeId, out Enabled enabled) == true)
            {
                if(enabled)
                {
                    if(_space.TryGetBody(node.TreeId, out IBody? body) == true)
                    {
                        this.DestroyFixtures(body, node, component);
                    }
                    else
                    {
                        _logger.Warning("{ClassName}::{MethodName} - Unable to destroy fixtures for node {NodeId} on tree {TreeId}. Body not found.", nameof(RigidEngine), nameof(OnDespawn), id.VhId, node.TreeId.VhId);
                    }
                }
                
            }
            else
            {
                _logger.Warning("{ClassName}::{MethodName} - Unable to destroy fixtures for node {NodeId} on tree {TreeId}. Tree not found.", nameof(RigidEngine), nameof(OnDespawn), id.VhId, node.TreeId.VhId);
            }
        }

        private void CreateFixtures(IBody body, Node node, Rigid rigid)
        {
            for(int i=0;i<rigid.Shapes.count; i++)
            {
                VhId rigidShapeId = node.Id.VhId.Create(i);
                _logger.Verbose("{ClassName}::{MethodName} - Creating fixture for tree {TreeId}; NodeId = {NodeId}, RigidShapeId = {RigidShapeId}", nameof(RigidEngine), nameof(CreateFixtures), body.Id.VhId, node.Id.VhId, rigidShapeId);
                body.Create(rigidShapeId, node.Id, rigid.Shapes[i], node.LocalLocation.Transformation);
            }
        }

        private void DestroyFixtures(IBody body, Node node, Rigid rigid)
        {
            for (int i = 0; i < rigid.Shapes.count; i++)
            {
                VhId rigidShapeId = node.Id.VhId.Create(i);
                _logger.Verbose("{ClassName}::{MethodName} - Destroying fixture for tree {TreeId}; NodeId = {NodeId}, RigidShapeId = {RigidShapeId}", nameof(RigidEngine), nameof(DestroyFixtures), body.Id.VhId, node.Id.VhId, rigidShapeId);
                body.Destroy(rigidShapeId);
            }
        }
    }
}
