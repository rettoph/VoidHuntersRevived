using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Game.Ships.Events
{
    public class Input_TractorBeamEmitter_Deselect : IInputData
    {
        public bool IsPredictable => true;

        public required VhId ShipVhId { get; init; }
        public required SocketVhId? AttachToSocketVhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Input_TractorBeamEmitter_Deselect, VhId, VhId, bool, SocketVhId>.Instance.Calculate(source, this.ShipVhId, this.AttachToSocketVhId.HasValue, this.AttachToSocketVhId!.Value);
        }
    }
}
