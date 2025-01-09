using System.Runtime.CompilerServices;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Domain.Physics.Extensions.tainicom.Aether.Physics2D.Dynamics;
using FixedMath64 = FixedMath.NET.Fix64;

namespace VoidHuntersRevived.Domain.Physics
{
    public class Body : IBody, IDisposable
    {
        private CollisionGroup _collisionCategories;
        private CollisionGroup _collidesWith;
        private readonly Space _space;
        private readonly Dictionary<FixtureId, Fixture> _fixtures;

        internal readonly AetherBody _aether;

        public ISpace? Space => this._space;

        public FixVector2 LocalCenter => this._aether.LocalCenter.AsFixVector2();

        public FixVector2 LinearVelocity => this._aether.LinearVelocity.AsFixVector2();

        public Fix64 AngularVelocity => (Fix64)this._aether.AngularVelocity;

        public EntityLocalId EntityLocalId { get; }

        public FixTransform2D Transform
        {
            get
            {
                var trans = this._aether.GetTransform();
                var output = new FixTransform2D(this._aether.Position.X, this._aether.Position.Y, trans.q.R, trans.q.i);

                return output;
            }
        }
        public FixVector2 Position
        {
            get => this._aether.Position.AsFixVector2();
            set
            {
                AetherVector2 aetherPosition = Unsafe.As<FixVector2, AetherVector2>(ref value);
                this._aether.SetPositionIgnoreContact(ref aetherPosition);
                this._aether.Awake = true;
            }
        }
        public Fix64 Rotation => this._aether.Rotation;

        public CollisionGroup CollisionCategories
        {
            get => this._collisionCategories;
            set
            {
                foreach (Fixture fixture in this._fixtures.Values)
                {
                    fixture._aether.CollisionCategories = (Category)value.Flags;
                }

                this._collisionCategories = value;
            }
        }
        public CollisionGroup CollidesWith
        {
            get => this._collidesWith;
            set
            {
                foreach (Fixture fixture in this._fixtures.Values)
                {
                    fixture._aether.CollidesWith = (Category)value.Flags;
                }

                this._collidesWith = value;
            }
        }

        public bool Enabled { get; private set; }
        public bool Awake => this._aether.Awake;
        public bool SleepingAllowed
        {
            get => this._aether.SleepingAllowed;
            set => this._aether.SleepingAllowed = value;
        }

        public Body(EntityLocalId entityLocalId, Space space)
        {
            this._space = space;
            this._aether = space._aether.CreateBody(AetherVector2.Zero, FixedMath64.Zero, BodyType.Dynamic);
            this._fixtures = [];
            this._aether.Tag = this;
            this._aether.AngularDamping = (Fix64)1m;
            this._aether.LinearDamping = (Fix64)0.25m;

            this.EntityLocalId = entityLocalId;
            this.Enabled = true;
        }

        public void SetTransform(FixTransform2D transform)
        {
            AetherTransform aetherTransform = Unsafe.As<FixTransform2D, AetherTransform>(ref transform);

            this._aether.SetTransformIgnoreContacts(aetherTransform);
            this._aether.Awake = true;
        }

        public void SetTransform(FixVector2 position, Fix64 rotation)
        {
            AetherVector2 aetherPosition = Unsafe.As<FixVector2, AetherVector2>(ref position);
            FixedMath64 fixedMathRotation = Unsafe.As<Fix64, FixedMath64>(ref rotation);

            this._aether.SetTransformIgnoreContacts(ref aetherPosition, fixedMathRotation);
            this._aether.Awake = true;
        }

        public void SetVelocity(FixVector2 linear, Fix64 angular)
        {
            AetherVector2 aetherLinear = Unsafe.As<FixVector2, AetherVector2>(ref linear);
            FixedMath64 fixedMathAngular = Unsafe.As<Fix64, FixedMath64>(ref angular);

            this._aether.LinearVelocity = aetherLinear;
            this._aether.AngularVelocity = fixedMathAngular;
        }

        public void ApplyAngularImpulse(Fix64 impulse)
        {
            this._aether.ApplyAngularImpulse(impulse);
        }

        public void ApplyForce(FixVector2 force, FixVector2 point)
        {
            AetherVector2 aetherForce = Unsafe.As<FixVector2, AetherVector2>(ref force);
            AetherVector2 aetherPoint = Unsafe.As<FixVector2, AetherVector2>(ref point);

            this._aether.ApplyForce(aetherForce, aetherPoint);
        }

        public void ApplyLinearImpulse(FixVector2 impulse)
        {
            this._aether.ApplyLinearImpulse(impulse.AsAetherVector2());
        }

        public IFixture Create(FixtureId id, Polygon polygon, FixMatrix transformation)
        {
            Fixture fixture = new(
                id,
                this,
                polygon.ToShape(transformation),
                (Category)this.CollisionCategories.Flags,
                (Category)this.CollidesWith.Flags);

            this._fixtures.Add(id, fixture);

            return fixture;
        }

        public void Destroy(FixtureId id)
        {
            if (this._fixtures.Remove(id, out Fixture? fixture))
            {
                fixture.Dispose();
            }
        }

        public void Dispose()
        {
            this.Enabled = false;
            this._space._aether.Remove(this._aether);
        }
    }
}