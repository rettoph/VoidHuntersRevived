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

        public override FixVector2 Position => (FixVector2)_body.Position;

        public override Fix64 Rotation => (Fix64)_body.Rotation;

        public override FixMatrix Transformation => _body.GetTransformation();

        public override void SetTransform(FixVector2 position, Fix64 rotation)
        {
            AetherVector2 aetherVector2 = (AetherVector2)position;
            _body.SetTransformIgnoreContacts(ref aetherVector2, rotation);
        }
    }
}
