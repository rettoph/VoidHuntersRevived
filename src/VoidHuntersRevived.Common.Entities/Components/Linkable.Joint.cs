using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public partial class Linkable
    {
        public class Joint
        {
            public Vector2 Position { get; set; }
            public float Rotation { get; set; }


            private Matrix? _asParentTransformation;
            public Matrix AsParentTransformation => _asParentTransformation ??= Matrix.CreateRotationZ(this.Rotation) * Matrix.CreateTranslation(this.Position.X, this.Position.Y, 0);

            private Matrix? _asChildTransformation;
            public Matrix AsChildTransformation => _asChildTransformation ??= Matrix.Invert(Matrix.CreateRotationZ(this.Rotation + MathHelper.Pi) * Matrix.CreateTranslation(this.Position.X, this.Position.Y, 0));
        }
    }
}
