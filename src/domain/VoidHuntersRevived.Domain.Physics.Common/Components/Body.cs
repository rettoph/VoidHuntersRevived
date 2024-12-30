using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Physics.Common.Components
{
    public unsafe struct Body(EntityLocalId localId, Fix64 rotation, FixTransform2D transform) : IEntityComponent, IHasMany<Fixture>
    {
        public static readonly Body Default = new(default, Fix64.Zero, FixTransform2D.Identity);

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

        public readonly EntityFilterId<Fixture> FixtureFilterId = EntityFilterId<Fixture>.Create<Body>(localId);
        EntityFilterId<Fixture> IHasMany<Fixture>.ChildrenFilterId => this.FixtureFilterId;

        public Body(EntityLocalId bodyLocalId, FixTransform2D transform) : this(bodyLocalId, transform.Rotation.Phase, transform)
        {

        }
        public Body(EntityLocalId bodyLocalId, FixVector2 position, Fix64 rotation) : this(bodyLocalId, rotation, new FixTransform2D(position, rotation))
        {

        }

        public void SetRotationTransform(Fix64 rotation, FixTransform2D transform)
        {
            _rotation = rotation;
            _transform = transform;
        }
    }
}
