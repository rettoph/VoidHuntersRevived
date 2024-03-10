using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.FixedPoint.Utilities;
using VoidHuntersRevived.Common.Physics.Components;

namespace VoidHuntersRevived.Common.Pieces.Components.Instance
{
    public struct Node : IEntityComponent
    {
        private bool _dirtyTransformation;
        private bool _dirtyXnaTransformation;
        private Location _localLocation;
        private FixMatrix _worldTransformation;
        private FixMatrix _transformation;
        private Matrix _xnaTransformation;

        public readonly EntityId Id;
        public readonly EntityId TreeId;

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

        public Node(EntityId id, EntityId treeId)
        {
            Id = id;
            TreeId = treeId;

            _dirtyTransformation = true;
            _dirtyXnaTransformation = true;
            _localLocation = new Location();
            _worldTransformation = FixMatrix.Identity;
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
