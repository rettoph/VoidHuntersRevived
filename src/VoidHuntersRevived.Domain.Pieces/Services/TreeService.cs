using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal partial class TreeService : ITreeService
    {
        private readonly IEntityService _entities;

        public TreeService(IEntityService entities)
        {
            _entities = entities;
        }

        public ref Node GetHead(in Tree tree)
        {
            return ref _entities.QueryById<Node>(tree.HeadId);
        }

        public ref Node GetHead(in EntityId treeId)
        {
            ref Tree tree = ref _entities.QueryById<Tree>(treeId);
            return ref this.GetHead(in tree);
        }

        public ref Node GetHead(in GroupIndex treeGroupIndex)
        {
            ref Tree tree = ref _entities.QueryByGroupIndex<Tree>(treeGroupIndex);
            return ref this.GetHead(in tree);
        }
    }
}
