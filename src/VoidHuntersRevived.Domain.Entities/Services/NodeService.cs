using Guppy.Common;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Messages;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class NodeService : INodeService
    {
        private readonly IBus _bus;

        public NodeService(IBus bus)
        {
            _bus = bus;
        }

        public void Attach(Degree outDegree, Degree inDegree)
        {
            var outId = outDegree.Node.EntityId;
            var inId = inDegree.Node.EntityId;

            Edge edge = new Edge(outDegree, inDegree);

            if (outDegree.Edge is not null && outDegree.Edge.InDegree != inDegree)
            { // The out degree already has another existing connection
                throw new NotImplementedException();
            }

            if (inDegree.Edge is not null && inDegree.Edge.OutDegree != outDegree)
            { // The in degree already has another existing connection
                throw new NotImplementedException();
            }

            if(inDegree.Node.Degrees.Any(x => x.Edge?.InDegree == x))
            { // The in node already has a defined in degree edge
                throw new NotImplementedException();
            }

            // The edge has been validated, add it and clean the nodes
            outDegree.Edge = edge;
            inDegree.Edge = edge;

            // At this point all downstream degrees should be recursively updated. Is there a better way?
            this.CleanLocalTransformationRecersive(inDegree, edge.LocalTransformation);

            _bus.Enqueue(new Created<Edge>(edge));
        }

        public void Detach(Degree degree)
        {
            throw new NotImplementedException();
        }

        private void CleanLocalTransformationRecersive(Degree inDegree, Matrix transformation)
        {
            foreach (Degree child in inDegree.Node.Degrees)
            {
                child.LocalTransformation = child.Configuration.Transformation * transformation;

                if(child.Edge is null)
                { // There are no further down stream edges to refresh
                    continue;
                }

                if(child.Edge.InDegree == inDegree)
                { // This is the same degree as the current input, no need to refresh again
                    continue;
                }

                this.CleanLocalTransformationRecersive(child.Edge.InDegree, child.LocalTransformation);
            }
        }
    }
}
