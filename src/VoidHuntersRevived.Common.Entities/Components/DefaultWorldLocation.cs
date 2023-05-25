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
        private AetherVector2 _position;
        private Fix64 _rotation;
        private FixedMatrix _transformation;

        public override AetherVector2 Position => _position;

        public override Fix64 Rotation => _rotation;

        public override FixedMatrix Transformation => _transformation;

        public DefaultWorldLocation()
        {
            this.SetTransform(AetherVector2.Zero, Fix64.Zero);
        }

        public override void SetTransform(AetherVector2 position, Fix64 rotation)
        {
            _position = position;
            _rotation = rotation;
            _transformation = FixedMatrix.CreateRotationZ(_rotation) * FixedMatrix.CreateTranslation(_position.X, _position.Y, Fix64.Zero);
        }
    }
}
