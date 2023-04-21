using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities
{
    public static class ParallelTypes
    {
        public static readonly ParallelType Pilot = ParallelType.GetOrRegister(nameof(Pilot));
        public static readonly ParallelType Ship = ParallelType.GetOrRegister(nameof(Ship));
        public static readonly ParallelType ShipPart = ParallelType.GetOrRegister(nameof(ShipPart));
        public static readonly ParallelType Chain = ParallelType.GetOrRegister(nameof(Chain));
    }
}
