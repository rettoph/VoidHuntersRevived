using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Physics
{
    public interface IBody
    {
        VhId Id { get; }

        ISpace? Space { get; }

        FixVector2 Position { get; }
        Fix64 Rotation { get; }
        FixMatrix Transformation { get; }

        FixVector2 LinearVelocity { get; }
        Fix64 AngularVelocity { get; }

        CollisionGroup CollisionCategories { get; set; }
        CollisionGroup CollidesWith { get; set; }

        bool Awake { get; }

        IFixture Create(VhId id, Polygon polygon, FixMatrix transformation);
        void Destroy(IFixture fixture);
        void Destroy(VhId id);

        void SetTransform(FixVector2 position, Fix64 rotation);
        void SetVelocity(FixVector2 linear, Fix64 angular);

        void ApplyAngularImpulse(Fix64 impulse);
        void ApplyLinearImpulse(FixVector2 impulse);
    }
}
