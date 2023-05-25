using FixedMath.NET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Common.Entities.Components
{
    internal class AetherBodyWorldLocation : WorldLocation
    {
        private readonly Body _body;

        public AetherBodyWorldLocation(Body body)
        {
            _body = body;
        }

        public override AetherVector2 Position => _body.Position;

        public override Fix64 Rotation => _body.Rotation;

        public override FixedMatrix Transformation => _body.GetTransformation();

        public override void SetTransform(AetherVector2 position, Fix64 rotation)
        {
            _body.SetTransformIgnoreContacts(ref position, rotation);
        }
    }
}
