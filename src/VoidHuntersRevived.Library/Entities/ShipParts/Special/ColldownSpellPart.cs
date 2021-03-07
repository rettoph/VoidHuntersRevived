using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Special
{
    /// <summary>
    /// Simple spell part with a built-in
    /// cooldown (defined by the ColldownSpellPartContext)
    /// </summary>
    public abstract class ColldownSpellPart<TSender, TArgs> : SpellPart<TSender, TArgs>
        where TSender : RigidShipPart
    {
    }
}
