using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Attributes
{
    public sealed class ShipPartContextTypeAttribute : Attribute
    {
        public readonly String Name;

        public ShipPartContextTypeAttribute(String name)
            => this.Name = name;
    }
}
