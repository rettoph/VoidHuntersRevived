﻿using Svelto.ECS;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.FixedPoint.Extensions;

namespace VoidHuntersRevived.Domain.Physics.Common.Components
{
    public struct Location : IEntityComponent
    {
        private Fix64 _rotation;
        private FixMatrix _transformation;

        public FixMatrix Transformation
        {
            get => _transformation;
            set
            {
                _transformation = value;
                _rotation = _transformation.Radians();
            }
        }

        public FixVector2 Position
        {
            set
            {
                _transformation.M41 = value.X;
                _transformation.M42 = value.Y;
            }
            get
            {
                return new FixVector2(_transformation.M41, _transformation.M42);
            }
        }
        public Fix64 Rotation
        {
            set
            {
                var val1 = Fix64.Cos(value);
                var val2 = Fix64.Sin(value);

                _transformation.M11 = val1;
                _transformation.M12 = val2;
                _transformation.M21 = -val2;
                _transformation.M22 = val1;

                _rotation = value;
            }
            get
            {
                return _rotation;
            }
        }

        public Location(FixVector2 position, Fix64 rotation)
        {
            _transformation = FixMatrix.Identity;

            this.Position = position;
            this.Rotation = rotation;
        }
        public Location()
        {
            _transformation = FixMatrix.Identity;
        }
    }
}
