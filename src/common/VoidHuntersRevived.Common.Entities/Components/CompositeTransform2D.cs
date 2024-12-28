using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct CompositeTransform2D : IEntityComponent
    {
        private bool _dirty;
        private FixVector2 _worldPosition;
        private FixMatrix2D _localTransform;
        private FixMatrix2D _value;

        public FixVector2 WorldPosition
        {
            set
            {
                _worldPosition = value;
                _dirty = true;
            }
        }

        public FixMatrix2D LocalTransformation
        {
            set
            {
                _localTransform = value;
                _dirty = true;
            }
        }

        public FixMatrix2D Value
        {
            get
            {
                if (_dirty)
                {
                    _value = _localTransform * _worldPosition;
                    _dirty = false;
                }

                return _value;
            }
        }
    }
}
