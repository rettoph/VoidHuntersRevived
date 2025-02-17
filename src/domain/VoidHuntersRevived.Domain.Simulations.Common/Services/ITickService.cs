using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Services
{
    public interface ITickService
    {
        /// <summary>
        ///  The last tick id to be given. AKA "current" tick
        /// </summary>
        int LastTickId { get; }

        /// <summary>
        /// The id of the next tick expected to be queued/dequeued
        /// </summary>
        int NextTickId { get; }

        /// <summary>
        /// Attempt to dequeue the next <see cref="Tick"/> instance
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tick"></param>
        /// <returns></returns>
        bool TryDequeue(int stepsSinceTick, [MaybeNullWhen(false)] out Tick tick);

        /// <summary>
        /// Attempt to enqueue a new <see cref="Tick"/>
        /// </summary>
        /// <param name="tick"></param>
        /// <returns></returns>
        EnqueueTickResponseEnum TryEnqueue(Tick tick);


        /// <summary>
        /// The <see cref="ITickService"/> has a say in whether or not 
        /// a step should be taken. This is because the tick service knows
        /// if there is a tick ready to be published or not - and is responsible
        /// for determining if we should slowly publish all available ticks or
        /// quickly publish them (like a client that just connected mid game and
        /// needs to syncronize)
        /// </summary>
        /// <param name="timeSinceLastStepLessThanStepInterval"></param>
        /// <returns></returns>
        bool ShouldStep(bool timeSinceLastStepLessThanStepInterval);

        /// <summary>
        /// Clear internal queues and reset the next tick id back to 0
        /// </summary>
        void Reset();
    }
}
