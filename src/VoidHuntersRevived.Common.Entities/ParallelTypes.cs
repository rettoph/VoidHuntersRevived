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
        public static readonly ParallelType Pilot = ParallelType.GetOrRegister("pilot");
        public static readonly ParallelType Ship = ParallelType.GetOrRegister("ship");
        public static readonly ParallelType ShipPart = ParallelType.GetOrRegister("ship_part");
        public static readonly ParallelType Chain = ParallelType.GetOrRegister("chain");
    }
}
