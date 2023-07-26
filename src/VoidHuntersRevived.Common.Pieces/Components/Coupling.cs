using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Coupling : IEntityComponent
    {
        public readonly SocketId SocketId;
        public FixMatrix Transformation;

        public Coupling(SocketId socketId)
        {
            SocketId = socketId;
        }
    }
}
