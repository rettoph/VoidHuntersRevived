using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    public struct Node(EntityLocalId localId, EntityLocalId treeLocalId) : IEntityComponent, IBelongsTo<Tree, Node>
    {
        private bool _dirtyTransformation = true;
        private FixTransform2D _localTransformation = new();
        private FixTransform2D _worldTransformation = FixTransform2D.Identity;
        private FixTransform2D _transformation;

        public readonly EntityLocalId LocalId = localId;
        public readonly EntityLocalId TreeLocalId = treeLocalId;

        public readonly EntityFilterId<Node> TreeFilterId => new(this.TreeLocalId.Value);
        EntityFilterId<Node> IBelongsTo<Tree, Node>.ParentFilterId => this.TreeFilterId;

        public FixTransform2D LocalTransformation => _localTransformation;
        public FixTransform2D Transformation
        {
            get
            {
                if (_dirtyTransformation == false)
                {
                    return _transformation;
                }

                _transformation = _localTransformation * _worldTransformation;
                _dirtyTransformation = false;

                return _transformation;
            }
        }

        public void SetWorldTransform(FixTransform2D worldTransform)
        {
            _worldTransformation = worldTransform;
            _dirtyTransformation = true;
        }

        public void SetLocationTransform(FixTransform2D localTransform)
        {
            _localTransformation = localTransform;
            _dirtyTransformation = true;
        }
    }
}
