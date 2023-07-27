using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Game.Pieces.Services
{
    internal sealed class SocketService : ISocketService, IQueryingEntitiesEngine
    {
        public EntitiesDB entitiesDB { get; set; } = null!;

        public void Ready()
        {
            // throw new NotImplementedException();
        }

        public SocketNode GetSocketNode(SocketId socketId)
        {
            var nodes = this.entitiesDB.QueryEntitiesAndIndex<Node>(socketId.NodeId.EGID, out uint index);
            var (sockets, _) = this.entitiesDB.QueryEntities<Sockets>(socketId.NodeId.EGID.groupID);


            return new SocketNode(ref sockets[index].Items[socketId.Index], ref nodes[index]);
        }
    }
}
