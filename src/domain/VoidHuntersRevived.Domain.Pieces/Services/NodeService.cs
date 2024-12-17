using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    public sealed class NodeService(IEntityQueryService entityQueryService) : INodeService
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        public ref Tree GetTree(in Node node)
        {
            return ref _entityQueryService.QueryByLocalId<Tree>(node.TreeLocalId);
        }

        public bool IsHead(in Node node)
        {
            return this.GetTree(node).HeadLocalId == node.LocalId;
        }

        public bool IsHead(in Node node, in GroupIndex treeGroupIndex)
        {
            if (_entityQueryService.TryQueryByGroupIndex<Tree>(treeGroupIndex, out Tree tree))
            {
                return tree.HeadLocalId == node.LocalId;
            }

            return false;
        }
    }
}
