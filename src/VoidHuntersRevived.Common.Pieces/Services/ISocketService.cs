using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface ISocketService
    {
        SocketNode GetSocketNode(SocketId socketId);
        bool TryGetSocketNode(SocketVhId socketVhId, out SocketNode socketNode);

        void Attach(ref Socket socket, EntityId treeId);
    }
}
