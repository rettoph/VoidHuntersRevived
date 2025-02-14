using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Extensions
{
    public static class IStepEventExtensions
    {
        public static Id<IStepEvent> CalculateId<T>(this T @event, VhId sourceId)
            where T : IStepEvent
        {
            return new Id<IStepEvent>(@event.CalculateHash(sourceId));
        }
    }
}
