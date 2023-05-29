using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Physics
{
    public interface IBody
    {
        ParallelKey EntityKey { get; }

        ISpace? Space { get; }

        FixVector2 Position { get; }
        Fix64 Rotation { get; }
        FixMatrix Transformation { get; }

        FixVector2 LinearVelocity { get; }
        Fix64 AngularVelocity { get; }

        IFixture Create(Polygon polygon, ParallelKey entityKey);
        void Destroy(IFixture fixture);

        void SetTransform(FixVector2 position, Fix64 rotation);
        void SetVelocity(FixVector2 linear, Fix64 angular);

        void ApplyAngularImpulse(Fix64 impulse);
        void ApplyLinearImpulse(FixVector2 impulse);
    }
}
