﻿using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal partial class TreeService : BasicEngine, ITreeService
    {
        private readonly IEntityService _entities;
        private readonly IBlueprintService _blueprints;

        public TreeService(IEntityService entities, IBlueprintService blueprints)
        {
            _entities = entities;
            _blueprints = blueprints;
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
