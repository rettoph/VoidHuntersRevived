using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Common.Services
{
    public interface ITickService
    {
        /// <summary>
        /// The id of the next tick expected to be queued/dequeued
        /// </summary>
        public int NextTickId { get; }

        /// <summary>
        /// Attempt to dequeue the next <see cref="Tick"/> instance
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tick"></param>
        /// <returns></returns>
        public bool TryDequeue(int stepsSinceTick, [MaybeNullWhen(false)] out Tick tick);

        /// <summary>
        /// Attempt to enqueue a new <see cref="Tick"/>
        /// </summary>
        /// <param name="tick"></param>
        /// <returns></returns>
        public EnqueueTickResponseEnum TryEnqueue(Tick tick);

        /// <summary>
        /// Clear internal queues and reset the next tick id back to 0
        /// </summary>
        public void Reset();
    }
}
