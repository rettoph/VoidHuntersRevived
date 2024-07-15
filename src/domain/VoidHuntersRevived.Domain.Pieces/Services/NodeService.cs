using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal sealed class NodeService : INodeService
    {
        private readonly IEntityQueryService _entityQueryService;

        public NodeService(IEntityQueryService entityQueryService)
        {
            _entityQueryService = entityQueryService;
        }

        public ref Tree GetTree(in Node node)
        {
            return ref _entityQueryService.QueryById<Tree>(node.TreeId);
        }

        public bool IsHead(in Node node)
        {
            return this.GetTree(node).HeadId == node.Id;
        }

        public bool IsHead(in Node node, in GroupIndex treeGroupIndex)
        {
            if (_entityQueryService.TryQueryByGroupIndex<Tree>(treeGroupIndex, out Tree tree))
            {
                return tree.HeadId == node.Id;
            }

            return false;
        }
    }
}
