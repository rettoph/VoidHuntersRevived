using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Physics.Common.Components
{
    public unsafe struct Body(Fix64 rotation, FixTransform2D transform) : IEntityComponent
    {
        public static readonly Body Default = new(Fix64.Zero, FixTransform2D.Identity);

        private Fix64 _rotation = rotation;
        private FixTransform2D _transform = transform;

        public FixVector2 Position
        {
            get => _transform.Position;
            set => _transform.Position = value;
        }
        public Fix64 Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                _transform.Rotation.Phase = value;
            }
        }
        public readonly FixTransform2D Transform => _transform;

        public Body(FixTransform2D transform) : this(transform.Rotation.Phase, transform)
        {

        }
        public Body(FixVector2 position, Fix64 rotation) : this(rotation, new FixTransform2D(position, rotation))
        {

        }

        public void SetRotationTransform(Fix64 rotation, FixTransform2D transform)
        {
            _rotation = rotation;
            _transform = transform;
        }
    }
}
