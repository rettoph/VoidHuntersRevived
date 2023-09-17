using Guppy.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal sealed class NodeEngine : BasicEngine,
        IReactOnAddEx<Node>,
        IReactOnRemoveEx<Node>
    {
        private readonly ISocketService _sockets;
        private readonly IEntityService _entities;
        private readonly ILogger _logger; 

        public NodeEngine(IEntityService entities, ISocketService sockets, ILogger logger)
        {
            _entities = entities;
            _sockets = sockets;
            _logger = logger;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Node> entities, ExclusiveGroupStruct groupID)
        {
            var (nodes, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                try
                {
                    this.SetLocalTransformation(ref nodes[index], groupID, index);

                    EntityId treeId = nodes[index].TreeId;
                    VhId nodeVhId = _entities.QueryByGroupIndex<EntityId>(groupID, index).VhId;

                    ref var filter = ref _entities.GetFilter<Node>(treeId, Tree.NodeFilterContextId);
                    filter.Add(ids[index], groupID, index);
                }
                catch(Exception ex)
                {
                    var id = _entities.QueryByGroupIndex<EntityId>(groupID, index);
                }
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Node> entities, ExclusiveGroupStruct groupID)
        {
            var (nodes, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                EntityId treeId = nodes[index].TreeId;
                VhId nodeVhId = _entities.QueryByGroupIndex<EntityId>(groupID, index).VhId;

                ref var filter = ref _entities.GetFilter<Node>(treeId, Tree.NodeFilterContextId);
                filter.Remove(ids[index], groupID);

                _logger.Verbose("{ClassName}::{MethodName} - Removed node {NodeId} from tree {TreeId}.", nameof(NodeEngine), nameof(Remove), nodeVhId.Value, treeId.VhId.Value);
            }
        }

        private void SetLocalTransformation(ref Node node, ExclusiveGroupStruct groupId, uint index)
        {
            _logger.Verbose("{ClassName}::{MethodName} - Preparing to set {LocalTransformation} for {Node} {NodeId}", nameof(NodeEngine), nameof(SetLocalTransformation), nameof(Node.LocalTransformation), nameof(Node), node.Id.VhId.Value);

            if (!_entities.TryQueryByGroupIndex<Coupling>(groupId, index, out Coupling coupling) || coupling.SocketId == SocketId.Empty)
            {
                node.LocalTransformation = FixMatrix.CreateTranslation(Fix64.Zero, Fix64.Zero, Fix64.Zero);
                return;
            }

            try
            {
                ref Plug plug = ref _entities.QueryByGroupIndex<Plug>(groupId, index);
                Socket socketNode = _sockets.GetSocket(coupling.SocketId);

                node.LocalTransformation = plug.Location.Transformation.Invert() * socketNode.LocalTransformation;
            }
            catch (Exception ex)
            {
                //TODO: Investigate what might cause this error
                // When this happens a valid piece gets eaten and destroyed
                // its the opposite of a dupe glitch
                // I can only replicate it by spam clicking the tractor beam selection buton and
                // moving the mouse randomly. It doesnt occurre very often
                // We set the transformation to zero so that the constructed rigid shape can still take form
                // Without this it will default all vertices to 0,0 and fail an assert
                node.LocalTransformation = FixMatrix.CreateTranslation(Fix64.Zero, Fix64.Zero, Fix64.Zero);

                var id = _entities.QueryByGroupIndex<EntityId>(groupId, index);
                _logger.Error(ex, "{ClassName}::{MethodName} - There was a fatal error attempting to set node transformation for node {NodeId}.", nameof(NodeEngine), nameof(SetLocalTransformation), id.VhId.Value);
                _entities.Despawn(id);
            }


            if(node.LocalTransformation == default)
            {

            }
        }
    }
}
