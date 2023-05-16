using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Common.Entities.Components
{
    internal class DefaultWorldLocation : WorldLocation
    {
        private Vector2 _position;
        private float _rotation;
        private Matrix _transformation;

        public override Vector2 Position => _position;

        public override float Rotation => _rotation;

        public override Matrix Transformation => _transformation;

        public DefaultWorldLocation()
        {
            this.SetTransform(Vector2.Zero, 0);
        }

        public override void SetTransform(Vector2 position, float rotation)
        {
            _position = position;
            _rotation = rotation;
            _transformation = Matrix.CreateRotationZ(_rotation) * Matrix.CreateTranslation(_position.X, _position.Y, 0);
        }
    }
}
