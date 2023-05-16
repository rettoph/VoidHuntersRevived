using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public override Vector2 Position => _body.Position;

        public override float Rotation => _body.Rotation;

        public override Matrix Transformation => _body.GetTransformation();

        public override void SetTransform(Vector2 position, float rotation)
        {
            _body.SetTransformIgnoreContacts(ref position, rotation);
        }
    }
}
