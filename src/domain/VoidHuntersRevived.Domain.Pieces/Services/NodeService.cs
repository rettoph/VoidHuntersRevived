using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
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
            return ref _entityQueryService.QueryById<Tree>(node.TreeId);
        }

        public bool IsHead(in Node node)
        {
            return this.GetTree(node).HeadLocalId == node.Id.ToLocalEntityId();
        }

        public bool IsHead(in Node node, in GroupIndex treeGroupIndex)
        {
            if (_entityQueryService.TryQueryByGroupIndex<Tree>(treeGroupIndex, out Tree tree))
            {
                return tree.HeadLocalId == node.Id.ToLocalEntityId();
            }

            return false;
        }
    }
}
