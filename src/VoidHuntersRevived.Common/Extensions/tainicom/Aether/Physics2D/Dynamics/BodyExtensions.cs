using FixedMath.NET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common;

namespace tainicom.Aether.Physics2D.Dynamics
{
    public static class BodyExtensions
    {
        public static void SetTransformIgnoreContacts(this Body body, AetherVector2 position, Fix64 angle)
        {
            body.SetTransformIgnoreContacts(ref position, angle);
        }

        public static FixedMatrix GetTransformation(this Body body)
        {
            return FixedMatrix.CreateRotationZ(body.Rotation) * FixedMatrix.CreateTranslation(body.Position.X, body.Position.Y, Fix64.Zero);
        }

        public static FixedMatrix GetCenterTransformation(this Body body)
        {
            return FixedMatrix.CreateTranslation(body.LocalCenter.X, body.LocalCenter.Y, Fix64.Zero) * body.GetTransformation();
        }

        public static FixedMatrix GetLocalCenterTransformation(this Body body)
        {
            return FixedMatrix.CreateTranslation(body.LocalCenter.X, body.LocalCenter.Y, Fix64.Zero);
        }
    }
}
