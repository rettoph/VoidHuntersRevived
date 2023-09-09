using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Entities.Options
{
    public struct SerializationOptions
    {
        public static SerializationOptions Default = new SerializationOptions()
        {
            Recursion = Recursion.All
        };

        public Recursion Recursion { get; init; }
    }
}
