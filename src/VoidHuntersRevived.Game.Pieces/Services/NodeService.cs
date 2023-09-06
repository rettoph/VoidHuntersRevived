﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VoidHuntersRevived.Common.Entities;
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

        public bool IsHead(in Node node, in GroupIndex treeGroupIndex)
        {
            if(_entities.TryQueryByGroupIndex<Tree>(treeGroupIndex, out Tree tree))
            {
                return tree.HeadId == node.Id;
            }

            return false;
        }
    }
}
