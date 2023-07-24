using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Common.Physics.Components
{
    public struct Location : IEntityComponent
    {
        public FixMatrix Transformation => FixMatrix.CreateRotationZ(Rotation) * FixMatrix.CreateTranslation(Position.X, Position.Y, Fix64.Zero);
        public FixVector2 Position;
        public Fix64 Rotation;

        public Location(FixVector2 position, Fix64 rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
        }
    }
}
