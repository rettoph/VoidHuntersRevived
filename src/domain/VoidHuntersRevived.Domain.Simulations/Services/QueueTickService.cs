using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public class QueueTickService : ITickService
    {
        private readonly int _lastEnqueuedTickId;
        private readonly Queue<Tick> _queue = [];

        public int NextTickId { get; private set; }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public bool TryDequeue(int stepsSinceTick, [MaybeNullWhen(false)] out Tick tick)
        {
            if (this._queue.TryDequeue(out tick) == true)
            {
                this.NextTickId++;
                return true;
            }

            return false;
        }

        public EnqueueTickResponseEnum TryEnqueue(Tick tick)
        {
            if (tick.Id != this._lastEnqueuedTickId + 1)
            {
                // Out of order error
                // Ticks must be queued in order within this service
                throw new NotImplementedException();
            }

            this._queue.Enqueue(tick);
            return EnqueueTickResponseEnum.Enqueued;
        }
    }
}
