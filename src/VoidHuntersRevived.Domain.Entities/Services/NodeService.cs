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

        public Edge Attach(Degree outDegree, Degree inDegree)
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
            this.CleanLocalTransformationRecersive(inDegree.Node, edge.LocalTransformation);

            _bus.Publish(new Created<Edge>(edge));

            return edge;
        }

        public void Detach(Edge edge)
        {
            edge.InDegree.Edge = null;
            edge.OutDegree.Edge = null;

            this.CleanLocalTransformationRecersive(edge.InDegree.Node, Matrix.Identity);

            _bus.Publish(new Destroyed<Edge>(edge));
        }

        private void CleanLocalTransformationRecersive(Node node, Matrix transformation)
        {
            node.LocalTransformation = transformation;

            foreach (Degree degree in node.Degrees)
            {
                degree.LocalTransformation = degree.Configuration.Transformation * transformation;

                if (degree.Edge is null)
                { // There are no further down stream edges to refresh
                    continue;
                }

                if(degree.Edge.InDegree == degree)
                { // This is the up degree, we do not want to clean down stream
                    continue;
                }

                this.CleanLocalTransformationRecersive(degree.Edge.InDegree.Node, degree.LocalTransformation);
            }
        }
    }
}
