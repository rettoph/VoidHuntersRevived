using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal sealed class NodeService : INodeService
    {
        private readonly IEntityService _entities;

        public NodeService(IEntityService entities)
        {
            _entities = entities;
        }

        public ref Tree GetTree(in Node node)
        {
            return ref _entities.QueryById<Tree>(node.TreeId);
        }

        public bool IsHead(in Node node)
        {
            return this.GetTree(node).HeadId == node.Id;
        }

        public bool IsHead(in Node node, in GroupIndex treeGroupIndex)
        {
            if (_entities.TryQueryByGroupIndex<Tree>(treeGroupIndex, out Tree tree))
            {
                return tree.HeadId == node.Id;
            }

            return false;
        }
    }
}
