using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Events
{
    public sealed class DestroyJointing : IData
    {
        public required ParallelKey Jointed { get; init; }
    }
}
