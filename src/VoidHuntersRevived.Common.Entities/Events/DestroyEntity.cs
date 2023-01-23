using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Events
{
    public sealed class DestroyEntity : IData
    {
        public ParallelKey EntityKey { get; set; }
    }
}
