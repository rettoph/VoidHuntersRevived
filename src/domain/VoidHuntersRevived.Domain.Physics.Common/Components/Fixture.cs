using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Physics.Common.Components
{
    public unsafe struct Fixture() : IEntityComponent
    {
        private Fix64? _worldRotation;

        public FixTransform2D LocalTransform { get; private set; } = FixTransform2D.Identity;
        public readonly FixVector2 LocalPosition => this.LocalTransform.Position;
        public Fix64 LocalRotation { get; private set; } = Fix64.Zero;


        public FixTransform2D WorldTransform { get; private set; }
        public FixVector2 WorldPosition => this.WorldTransform.Position;
        public Fix64 WorldRotation => _worldRotation ??= this.WorldTransform.Rotation.Phase;

        public void SetLocalRotationTransform(Fix64 rotation, FixTransform2D transform)
        {
            this.LocalRotation = rotation;
            this.LocalTransform = transform;
        }

        public void SetBodyTransform(FixTransform2D bodyTransform)
        {
            this.WorldTransform = this.LocalTransform * bodyTransform;
            _worldRotation = null;
        }
    }
}
