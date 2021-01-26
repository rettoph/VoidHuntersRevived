using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Builder.Attributes
{
    public class ShipPartContextBuilderServiceAttribute : Attribute
    {
        public readonly String Title;
        public readonly UInt32 Order;

        public ShipPartContextBuilderServiceAttribute(String title, UInt32 order)
        {
            Title = title;
            Order = order;
        }
    }
}
