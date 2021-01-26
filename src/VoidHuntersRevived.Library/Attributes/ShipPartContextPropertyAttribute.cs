using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Attributes
{
    public enum ShipPartContextPropertyType
    {
        Single,
        Vector2,
        Radian,
        String,
        Int32,
        Color,
        Boolean
    }

    public class ShipPartContextPropertyAttribute : Attribute
    {
        public readonly String Name;
        public readonly String Description;
        public readonly ShipPartContextPropertyType Type;

        public ShipPartContextPropertyAttribute(String name, String description, ShipPartContextPropertyType type)
        {
            this.Name = name;
            this.Description = description;
            this.Type = type;
        }
    }
}
