using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.FixedPoint.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Physics.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    public struct Node(EntityId id, EntityLocalId treeLocalId) : IEntityComponent, IBelongsTo<Tree, Node>
    {
        private bool _dirtyTransformation = true;
        private bool _dirtyXnaTransformation = true;
        private Location _localLocation = new();
        private FixMatrix _worldTransformation = FixMatrix.Identity;
        private FixMatrix _transformation;
        private Matrix _xnaTransformation;

        public readonly EntityId Id = id;
        public readonly EntityLocalId TreeLocalId = treeLocalId;

        public readonly EntityFilterId<Node> TreeFilterId => new(this.TreeLocalId.Value);
        EntityFilterId<Node> IBelongsTo<Tree, Node>.ParentFilterId => this.TreeFilterId;

        public Location LocalLocation => _localLocation;
        public FixMatrix Transformation
        {
            get
            {
                if (_dirtyTransformation == false)
                {
                    return _transformation;
                }

                _transformation = FixMatrixHelper.FastMultiplyTransformations(LocalLocation.Transformation, _worldTransformation);
                _dirtyTransformation = false;

                return _transformation;
            }
        }
        public Matrix XnaTransformation
        {
            get
            {
                if (_dirtyXnaTransformation == false)
                {
                    return _xnaTransformation;
                }

                _xnaTransformation = Transformation.ToTransformationXnaMatrix();
                _dirtyXnaTransformation = false;

                return _xnaTransformation;
            }
        }



        public void WorldTransform(FixMatrix world)
        {
            _worldTransformation = world;
            _dirtyTransformation = true;
            _dirtyXnaTransformation = true;
        }

        public void SetLocationTransformation(FixMatrix transformation)
        {
            _localLocation.Transformation = transformation;
            _dirtyTransformation = true;
            _dirtyXnaTransformation = true;
        }
    }
}
