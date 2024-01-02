using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Physics
{
    public interface IBody
    {
        EntityId Id { get; }

        ISpace? Space { get; }

        FixVector2 LocalCenter { get; }
        FixVector2 Position { get; }
        Fix64 Rotation { get; }
        FixMatrix Transformation { get; }

        FixVector2 LinearVelocity { get; }
        Fix64 AngularVelocity { get; }

        CollisionGroup CollisionCategories { get; set; }
        CollisionGroup CollidesWith { get; set; }

        bool Enabled { get; }
        bool Awake { get; }
        bool SleepingAllowed { get; set; }

        IFixture Create(VhId id, EntityId entityId, Polygon polygon, FixMatrix transformation);
        void Destroy(IFixture fixture);
        void Destroy(VhId id);

        void SetTransform(FixVector2 position, Fix64 rotation);
        void SetVelocity(FixVector2 linear, Fix64 angular);

        void ApplyAngularImpulse(Fix64 impulse);
        void ApplyLinearImpulse(FixVector2 impulse);

        /// <summary>
        /// Apply a force at a world point. If the force is not
        /// applied at the center of mass, it will generate a torque and
        /// affect the angular velocity. This wakes up the body.
        /// </summary>
        /// <param name="force">The world force vector, usually in Newtons (N).</param>
        /// <param name="point">The world position of the point of application.</param>
        void ApplyForce(FixVector2 force, FixVector2 point);
    }
}
