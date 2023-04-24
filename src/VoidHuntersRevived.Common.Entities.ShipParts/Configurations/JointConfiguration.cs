using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Configurations
{
    public sealed class JointConfiguration
    {
        private Vector2 _position;
        private float _rotation;

        public Vector2 Position
        {
            get => _position; 
            set
            {
                _position = value;
                this.CleanTransformation();
            }
        }
        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                this.CleanTransformation();
            }
        }

        public Matrix Transformation;

        public JointConfiguration() : this(Vector2.Zero, 0f)
        {

        }
        public JointConfiguration(Vector2 position, float rotation)
        {
            _position = position;
            _rotation = rotation;

            this.CleanTransformation();
        }

        private void CleanTransformation()
        {
            this.Transformation = Matrix.CreateRotationZ(this.Rotation) * Matrix.CreateTranslation(this.Position.X, this.Position.Y, 0);
        }
    }
}
