using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Physics.Common.Components
{
    public unsafe struct Fixture(EntityLocalId bodyLocalId) : IEntityComponent, IBelongsTo<Body, Fixture>
    {
        private Fix64? _worldRotation;

        public FixTransform2D LocalTransform { get; private set; } = FixTransform2D.Identity;
        public readonly FixVector2 LocalPosition => this.LocalTransform.Position;
        public Fix64 LocalRotation { get; private set; } = Fix64.Zero;


        public FixTransform2D WorldTransform { get; private set; }
        public FixVector2 WorldPosition => this.WorldTransform.Position;
        public Fix64 WorldRotation => _worldRotation ??= this.WorldTransform.Rotation.Phase;

        public readonly EntityFilterId<Fixture> BodyFilterId = EntityFilterId<Fixture>.Create<Body>(bodyLocalId);
        EntityFilterId<Fixture> IBelongsTo<Body, Fixture>.ParentFilterId => this.BodyFilterId;

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
