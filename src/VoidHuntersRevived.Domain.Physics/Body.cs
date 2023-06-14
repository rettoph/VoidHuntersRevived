using System.Runtime.CompilerServices;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Common;
using FixedMath64 = FixedMath.NET.Fix64;

namespace VoidHuntersRevived.Domain.Physics
{
    public class Body : IBody
    {
        private readonly AetherBody _aether;
        private readonly HashSet<Fixture> _fixtures;

        public ISpace? Space { get; internal set; }

        public FixVector2 Position => _aether.Position.AsFixVector2();

        public Fix64 Rotation => (Fix64)_aether.Rotation;

        public FixVector2 LinearVelocity => _aether.LinearVelocity.AsFixVector2();

        public Fix64 AngularVelocity => (Fix64)_aether.AngularVelocity;

        public VhId Id { get; }

        public FixMatrix Transformation => FixMatrix.CreateRotationZ(this.Rotation) * FixMatrix.CreateTranslation(this.Position.X, this.Position.Y, Fix64.Zero);

        public Body(VhId id)
        {
            _aether = new AetherBody()
            {
                BodyType = tainicom.Aether.Physics2D.Dynamics.BodyType.Dynamic
            };
            _fixtures = new HashSet<Fixture>();
            _aether.Tag = this;

            this.Id = id;
        }

        public void SetTransform(FixVector2 position, Fix64 rotation)
        {
            AetherVector2 aetherPosition = Unsafe.As<FixVector2, AetherVector2>(ref position);
            FixedMath64 fixedMathRotation = Unsafe.As<Fix64, FixedMath64>(ref rotation);

            _aether.SetTransformIgnoreContacts(ref aetherPosition, fixedMathRotation);
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

        public IFixture Create(Polygon polygon, Guid id)
        {
            Fixture fixture = new Fixture(this, polygon, id);
            fixture.AddToBody(_aether);
            _fixtures.Add(fixture);

            return fixture;
        }

        public void Destroy(IFixture fixture)
        {
            if (fixture is not Fixture casted)
            {
                return;
            }

            if(!_fixtures.Remove(casted))
            {
                return;
            }

            casted.RemoveFromBody(_aether);
        }

        internal void AddToWorld(AetherWorld world)
        {
            world.Add(_aether);
        }

        internal void RemoveFromWorld(AetherWorld world)
        {
            world.Remove(_aether);
        }
    }
}
