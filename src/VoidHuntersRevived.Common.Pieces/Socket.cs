using Microsoft.Xna.Framework;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.FixedPoint.Utilities;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Pieces
{
    public readonly struct Socket
    {
        public readonly Node Node;

        public readonly SocketId Id;
        public readonly Location Location;

        public FixMatrix LocalTransformation => FixMatrixHelper.FastMultiplyTransformations(this.Location.Transformation, Node.LocalLocation.Transformation);
        public FixMatrix Transformation => FixMatrixHelper.FastMultiplyTransformations(this.Location.Transformation, Node.Transformation);

        //public Matrix XnaLocalTransformation => FixMatrixHelper.FastMultiplyTransformationsToXnaMatrix(this.Location.Transformation, Node.Transformation);
        //public Matrix XnaTransformation => FixMatrixHelper.FastMultiplyTransformationsToXnaMatrix(this.Location.Transformation, Node.Transformation);


        public Socket(Node node, SocketId socketId, Location location)
        {
            this.Node = node;
            this.Id = socketId;
            this.Location = location;
        }
    }
}
