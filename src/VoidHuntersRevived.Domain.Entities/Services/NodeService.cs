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

        public Link Attach(Joint child, Joint parent)
        {
            Link link = new Link(child, parent);

            if (parent.Link is not null && parent.Link.Child != child)
            { // The parent joint already has another existing connection
                throw new NotImplementedException();
            }

            if (child.Link is not null && child.Link.Parent != parent)
            { // The child joint already has another existing connection
                throw new NotImplementedException();
            }

            if(child.Node.Joints.Any(x => x.Link?.Child == x))
            { // The child node already has a defined parent joint link
                throw new NotImplementedException();
            }

            // The link has been validated, add it and clean the nodes
            child.Link = link;
            parent.Link = link;

            // At this point all down joints should be recursively updated. Is there a better way?
            this.CleanLocalTransformationRecersive(child.Node, link.LocalTransformation);

            _bus.Publish(new Created<Link>(link));

            return link;
        }

        public void Detach(Link link)
        {
            link.Parent.Link = null;
            link.Child.Link = null;

            this.CleanLocalTransformationRecersive(link.Child.Node, Matrix.Identity);

            _bus.Publish(new Destroyed<Link>(link));
        }

        private void CleanLocalTransformationRecersive(Node node, Matrix transformation)
        {
            node.LocalTransformation = transformation;

            foreach (Joint joint in node.Joints)
            {
                joint.LocalTransformation = joint.Configuration.Transformation * transformation;

                if (joint.Link is null)
                { // There are no further down stream links to refresh
                    continue;
                }

                if(joint.Link.Child == joint)
                { // Only clean parent joints
                    continue;
                }

                this.CleanLocalTransformationRecersive(joint.Link.Child.Node, joint.Link.LocalTransformation);
            }
        }
    }
}
