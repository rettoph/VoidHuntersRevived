using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using static VoidHuntersRevived.Library.Entities.Ship;

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
        public Ship Ship { get; set; }

        public override Body CreateBody(World world)
        {
            var body = base.CreateBody(world);
            body.LinearDamping = 1f;
            body.AngularDamping = 1f;
            FixtureFactory.AttachCircle(1f, 1f, body);

            return body;
        }
    }
}
