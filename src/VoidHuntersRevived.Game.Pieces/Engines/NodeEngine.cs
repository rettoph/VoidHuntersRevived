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
using VoidHuntersRevived.Game.Pieces.Events;

namespace VoidHuntersRevived.Game.Pieces.Engines
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
                this.SetLocalTransformation(ref nodes[index], groupID, index);

                EntityId treeId = nodes[index].TreeId;
                VhId nodeVhId = _entities.GetId(ids[index], groupID).VhId;

                ref var filter = ref _entities.GetFilter<Node>(treeId, Tree.NodeFilterContextId);
                filter.Add(ids[index], groupID, index);
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Node> entities, ExclusiveGroupStruct groupID)
        {
            var (nodes, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                EntityId treeId = nodes[index].TreeId;
                VhId nodeVhId = _entities.GetId(ids[index], groupID).VhId;

                ref var filter = ref _entities.GetFilter<Node>(treeId, Tree.NodeFilterContextId);
                filter.Remove(ids[index], groupID);
            }
        }

        private void SetLocalTransformation(ref Node node, ExclusiveGroupStruct groupId, uint index)
        {
            if (!_entities.TryQueryByGroupIndex<Coupling>(groupId, index, out Coupling coupling) || coupling.SocketId == SocketId.Empty)
            {
                node.LocalTransformation = FixMatrix.CreateTranslation(Fix64.Zero, Fix64.Zero, Fix64.Zero);
                return;
            }
            var id = _entities.QueryByGroupIndex<EntityId>(groupId, index);

            ref Plug plug = ref _entities.QueryByGroupIndex<Plug>(groupId, index);
            SocketNode socketNode = _sockets.GetSocketNode(coupling.SocketId);

            node.LocalTransformation = plug.Location.Transformation.Invert() * socketNode.LocalTransformation;
        }
    }
}
