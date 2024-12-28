using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct WorldTransform2D : IEntityComponent
    {
        private bool _dirty;
        private FixTransform2D _worldTransform;
        private FixTransform2D _localTransform;
        private FixTransform2D _value;

        public FixTransform2D WorldTransform
        {
            get => _worldTransform;
            set
            {
                _worldTransform = value;
                _dirty = true;
            }
        }

        public FixTransform2D LocalTransformation
        {
            get => _localTransform;
            set
            {
                _localTransform = value;
                _dirty = true;
            }
        }

        public FixTransform2D Value
        {
            get
            {
                if (_dirty)
                {
                    _value = _localTransform * _worldTransform;
                    _dirty = false;
                }

                return _value;
            }
        }
    }
}
