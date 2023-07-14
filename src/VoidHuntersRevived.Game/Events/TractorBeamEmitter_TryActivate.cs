using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Game.Events
{
    public class TractorBeamEmitter_TryActivate : IInputData
    {
        public static readonly VhId NameSpace = new VhId("2336DDC1-1F88-4B5A-A00A-D2DEFE3FCE52");

        public required VhId ShipVhId { get; init; }
        public required VhId TargetVhId { get; init; }
    }
}
