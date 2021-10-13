using Guppy.Extensions.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Components.Entities.Ships;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Components.Entities.Ships
{
    internal sealed class ShipTractorBeamDrawComponent : FrameableDrawComponent<Ship>
    {
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            ShipTractorBeamComponent tractorBeam = this.Entity.Components.Get<ShipTractorBeamComponent>();
            ConnectionNode targetNode = tractorBeam.GetConnectionNodeTarget();

            if(targetNode != default)
            {
                this.primitiveBatch.TraceCircle(
                    Color.White,
                    targetNode.Owner.CalculateWorldPoint(targetNode.LocalPosition),
                    0.25f,
                    25);
            }
        }
    }
}
