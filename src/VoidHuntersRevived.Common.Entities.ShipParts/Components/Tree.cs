namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public class Tree
    {
        public readonly int EntityId;
        public readonly IList<Node> Nodes;
        public Node? Head => this.Nodes.Any() ? this.Nodes[0] : null;

        public Tree(int entityId)
        {
            this.EntityId = entityId;
            this.Nodes = new List<Node>();
        }
    }
}
