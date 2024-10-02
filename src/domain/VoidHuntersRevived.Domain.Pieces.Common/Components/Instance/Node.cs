using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.FixedPoint.Extensions;
using VoidHuntersRevived.Common.FixedPoint.Utilities;
using VoidHuntersRevived.Domain.Physics.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components.Instance
{
    public struct Node(EntityId id, EntityId treeId) : IEntityComponent
    {
        private bool _dirtyTransformation = true;
        private bool _dirtyXnaTransformation = true;
        private Location _localLocation = new Location();
        private FixMatrix _worldTransformation = FixMatrix.Identity;
        private FixMatrix _transformation;
        private Matrix _xnaTransformation;

        public readonly EntityId Id = id;
        public readonly EntityId TreeId = treeId;

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
