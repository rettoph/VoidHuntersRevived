using Guppy.Attributes;
using Serilog;
using Svelto.ECS;
using System.Text.RegularExpressions;
using System;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using Guppy.Common.Collections;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Common.Pieces.Events;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Physics.Components;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal sealed class NodeEngine : BasicEngine,
        IOnSpawnEngine<Node>,
        IOnDespawnEngine<Node>,
        IStepEngine<Step>
    {
        private readonly ISocketService _sockets;
        private readonly IEntityService _entities;
        private readonly ILogger _logger;
        private readonly DictionaryQueue<EntityId, VhId> _dirtyTrees;

        public NodeEngine(IEntityService entities, ISocketService sockets, ILogger logger)
        {
            _entities = entities;
            _sockets = sockets;
            _logger = logger;
            _dirtyTrees = new DictionaryQueue<EntityId, VhId>();
        }

        public string name { get; } = nameof(NodeEngine);

        public void OnSpawn(EntityId id, ref Node node, in GroupIndex groupIndex)
        {
            _logger.Verbose("{ClassName}::{MethodName} - EntityId = {EntityId}", nameof(NodeEngine), nameof(OnSpawn), id.VhId);

            ref var filter = ref _entities.GetFilter<Node>(node.TreeId, Tree.NodeFilterContextId);
            filter.Add(id, groupIndex);

            ref VhId dirtyEventId = ref _dirtyTrees.GetOrEnqueue(node.TreeId, out bool alreadyDirty);
            dirtyEventId = alreadyDirty
                ? HashBuilder<IReactOnAddEx<Node>, VhId, VhId>.Instance.Calculate(dirtyEventId, node.Id.VhId)
                : HashBuilder<IReactOnAddEx<Node>, VhId>.Instance.Calculate(node.Id.VhId);

            this.SetLocalTransformation(ref node, groupIndex);
        }

        public void OnDespawn(EntityId id, ref Node node, in GroupIndex groupIndex)
        {
            _logger.Verbose("{ClassName}::{MethodName} - EntityId = {EntityId}", nameof(NodeEngine), nameof(OnDespawn), id.VhId);

            ref var filter = ref _entities.GetFilter<Node>(node.TreeId, Tree.NodeFilterContextId);
            filter.Remove(id.EGID);

            ref VhId dirtyEventId = ref _dirtyTrees.GetOrEnqueue(node.TreeId, out bool alreadyDirty);
            dirtyEventId = alreadyDirty
                ? HashBuilder<IReactOnRemoveEx<Node>, VhId, VhId>.Instance.Calculate(dirtyEventId, node.Id.VhId)
                : HashBuilder<IReactOnRemoveEx<Node>, VhId>.Instance.Calculate(node.Id.VhId);
        }

        public void Step(in Step param)
        {
            while(_dirtyTrees.TryDequeue(out EntityId dirtyTreeId, out VhId cleaTreeEventSender))
            {
                if(_entities.IsSpawned(dirtyTreeId))
                {
                    this.Simulation.Publish(cleaTreeEventSender, new Tree_Clean()
                    {
                        TreeId = dirtyTreeId.VhId
                    });
                }
            }
        }

        private void SetLocalTransformation(ref Node node, in GroupIndex groupIndex)
        {
            _logger.Verbose("{ClassName}::{MethodName} - Preparing to set {LocalTransformation} for {Node} {NodeId}", nameof(NodeEngine), nameof(SetLocalTransformation), nameof(Node.LocalLocation), nameof(Node), node.Id.VhId.Value);

            if (!_entities.TryQueryByGroupIndex<Coupling>(groupIndex, out Coupling coupling) || coupling.SocketId == SocketId.Empty)
            {
                node.SetLocationTransformation(FixMatrix.Identity);
                return;
            }

            try
            {
                ref Plug plug = ref _entities.QueryByGroupIndex<Plug>(groupIndex);
                Socket socketNode = _sockets.GetSocket(coupling.SocketId);

                node.SetLocationTransformation(plug.Location.Transformation.Invert() * socketNode.LocalTransformation);

                ref Location worldLocation = ref _entities.QueryById<Location>(node.TreeId);
                node.WorldTransform(worldLocation.Transformation);
            }
            catch (Exception ex)
            {
                // TODO: Investigate what might cause this error
                // When this happens a valid piece gets eaten and destroyed
                // its the opposite of a dupe glitch
                // I can only replicate it by spam clicking the tractor beam selection button and
                // moving the mouse randomly. It doesnt occurre very often
                // We set the transformation to zero so that the constructed rigid shape can still take form
                // Without this it will default all vertices to 0,0 and fail an assert
                node.SetLocationTransformation(FixMatrix.Identity);

                var id = _entities.QueryByGroupIndex<EntityId>(groupIndex);
                _logger.Error(ex, "{ClassName}::{MethodName} - There was a fatal error attempting to set node transformation for node {NodeId}.", nameof(NodeEngine), nameof(SetLocalTransformation), id.VhId.Value);
                _entities.Despawn(id);
            }
        }
    }
}
