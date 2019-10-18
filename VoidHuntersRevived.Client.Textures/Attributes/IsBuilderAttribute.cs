using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Client.Textures.Attributes
{
    class IsBuilderAttribute : GuppyAttribute
    {
        public readonly Type Type;

        public IsBuilderAttribute(Type type, Int32 priority = 100) : base(priority)
        {
            this.Type = type;
        }
    }
}
