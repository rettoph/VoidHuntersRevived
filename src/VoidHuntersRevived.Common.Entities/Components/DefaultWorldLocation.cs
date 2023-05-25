using FixedMath.NET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Common.Entities.Components
{
    internal class DefaultWorldLocation : WorldLocation
    {
        private FixVector2 _position;
        private Fix64 _rotation;
        private FixMatrix _transformation;

        public override FixVector2 Position => _position;

        public override Fix64 Rotation => _rotation;

        public override FixMatrix Transformation => _transformation;

        public DefaultWorldLocation()
        {
            this.SetTransform(FixVector2.Zero, Fix64.Zero);
        }

        public override void SetTransform(FixVector2 position, Fix64 rotation)
        {
            _position = position;
            _rotation = rotation;
            _transformation = FixMatrix.CreateRotationZ(_rotation) * FixMatrix.CreateTranslation(_position.X, _position.Y, Fix64.Zero);
        }
    }
}
