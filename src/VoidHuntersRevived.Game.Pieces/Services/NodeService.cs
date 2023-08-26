using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Game.Pieces.Services
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
    }
}
