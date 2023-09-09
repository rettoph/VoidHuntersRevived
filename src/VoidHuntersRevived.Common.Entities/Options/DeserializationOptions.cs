using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Options
{
    public unsafe struct DeserializationOptions
    {
        public VhId Seed { get; init; }
        public TeamId TeamId { get; init; }
        public VhId Owner { get; init; }
    }
}
