using System.Runtime.CompilerServices;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Common;
using FixedMath64 = FixedMath.NET.Fix64;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Dynamics;

namespace VoidHuntersRevived.Domain.Physics
{
    public class Body : IBody, IDisposable
    {
        private CollisionGroup _collisionCategories;
        private CollisionGroup _collidesWith;
        private readonly Space _space;
        private readonly Dictionary<VhId, Fixture> _fixtures;

        internal readonly AetherBody _aether;

        public ISpace? Space => _space;

        public FixVector2 Position => _aether.Position.AsFixVector2();

        public Fix64 Rotation => (Fix64)_aether.Rotation;

        public FixVector2 LinearVelocity => _aether.LinearVelocity.AsFixVector2();

        public Fix64 AngularVelocity => (Fix64)_aether.AngularVelocity;

        public VhId Id { get; }

        public FixMatrix Transformation => FixMatrix.CreateRotationZ(this.Rotation) * FixMatrix.CreateTranslation(this.Position.X, this.Position.Y, Fix64.Zero);

        public CollisionGroup CollisionCategories
        {
            get => _collisionCategories;
            set
            {
                foreach(Fixture fixture in _fixtures.Values)
                {
                    fixture._aether.CollisionCategories = (Category)value.Flags;
                }

                _collisionCategories = value;
            }
        }
        public CollisionGroup CollidesWith
        {
            get => _collidesWith;
            set
            {
                foreach (Fixture fixture in _fixtures.Values)
                {
                    fixture._aether.CollidesWith = (Category)value.Flags;
                }

                _collidesWith = value;
            }
        }

        public bool Awake => _aether.Awake;

        public Body(Space space, VhId id)
        {
            _space = space;
            _aether = space._aether.CreateBody(AetherVector2.Zero, FixedMath64.Zero, BodyType.Dynamic);
            _fixtures = new Dictionary<VhId, Fixture>();
            _aether.Tag = this;

            this.Id = id;
        }

        public void SetTransform(FixVector2 position, Fix64 rotation)
        {
            AetherVector2 aetherPosition = Unsafe.As<FixVector2, AetherVector2>(ref position);
            FixedMath64 fixedMathRotation = Unsafe.As<Fix64, FixedMath64>(ref rotation);

            _aether.SetTransformIgnoreContacts(ref aetherPosition, fixedMathRotation);
            _aether.Awake = true;
        }

        public void SetVelocity(FixVector2 linear, Fix64 angular)
        {
            AetherVector2 aetherLinear = Unsafe.As<FixVector2, AetherVector2>(ref linear);
            FixedMath64 fixedMathAngular = Unsafe.As<Fix64, FixedMath64>(ref angular);

            _aether.LinearVelocity = aetherLinear;
            _aether.AngularVelocity = fixedMathAngular;
        }

        public void ApplyAngularImpulse(Fix64 impulse)
        {
            _aether.ApplyAngularImpulse(impulse);
        }

        public void ApplyLinearImpulse(FixVector2 impulse)
        {
            _aether.ApplyLinearImpulse(impulse.AsAetherVector2());
        }

        public IFixture Create(VhId id, Polygon polygon, FixMatrix transformation)
        {
            Fixture fixture = new Fixture(
                id,
                this,
                polygon.ToShape(transformation), 
                (Category)this.CollisionCategories.Flags,
                (Category)this.CollidesWith.Flags);

            _fixtures.Add(id, fixture);

            return fixture;
        }

        public void Destroy(IFixture fixture)
        {
            if (fixture is not Fixture casted)
            {
                return;
            }

            if(!_fixtures.Remove(casted.Id))
            {
                return;
            }

            casted.Dispose();
        }

        public void Destroy(VhId id)
        {
            if(_fixtures.Remove(id, out Fixture? fixture))
            {
                fixture.Dispose();
            }
        }

        public void Dispose()
        {
            _space._aether.Remove(_aether);
        }
    }
}
