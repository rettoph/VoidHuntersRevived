using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface ISocketService
    {
        SocketNode GetSocketNode(SocketId socketId);
        bool TryGetSocketNode(SocketVhId socketVhId, out SocketNode socketNode);

        void Attach(SocketNode socketNode, Tree tree);
        bool TryDetach(EntityId couplingId, EntityInitializerDelegate initializer, out EntityId cloneId);

        ref EntityFilterCollection GetCouplingFilter(SocketId socketId);
    }
}
