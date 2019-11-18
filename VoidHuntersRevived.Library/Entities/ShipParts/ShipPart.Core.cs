using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// The core ShipPart class.
    /// 
    /// This will primarily augment the base
    /// FarseerEntity class.
    /// </summary>
    public partial class ShipPart : FarseerEntity
    {
        public override Body CreateBody(World world)
        {
            var body = base.CreateBody(world);
            FixtureFactory.AttachCircle(1f, 1f, body);

            return body;
        }
    }
}
