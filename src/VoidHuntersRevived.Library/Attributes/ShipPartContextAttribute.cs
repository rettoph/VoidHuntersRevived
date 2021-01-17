using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Attributes
{
    public class ShipPartContextAttribute : Attribute
    {
        public readonly String Name;
        public readonly String Description;

        public ShipPartContextAttribute(String name, String description)
        {
            this.Name = name;
            this.Description = description;
        }
    }
}
