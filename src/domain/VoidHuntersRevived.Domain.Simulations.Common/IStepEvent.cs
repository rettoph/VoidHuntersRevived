using System.Runtime.CompilerServices;
using Guppy.Core.Messaging.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    /// <summary>
    /// Represents immutable event data that
    /// occured durring the current strategy step
    /// </summary>
    public interface IStepEvent
    {
        /// <summary>
        /// Indicates an event is predictable and can be invoked before
        /// any lockstep confirmation is recieved.
        /// </summary>
        bool IsPredictable { get; }

        /// <summary>
        /// <para>
        /// Indicates an event that should only be invoked on the source
        /// simulation. It is not shared or confirmed by the predictive simulation.
        /// There is an implicit assumtion that syncing will happen despite the 
        /// unshared event. (or is not required on the server - such as partical
        /// related events)
        /// </para>
        /// <para>
        /// If false, the event may be enqueued with <see cref="IStrategy.Enqueue(IStepEvent)"/>
        /// </para>
        /// </summary>
        bool IsPrivate => false;

        void Publish(VhId sourceId, IMessageBus messageBus);
        void Revert(VhId sourceId, IMessageBus messageBus);

        VhId CalculateHash(in VhId sourceId);
    }

    public interface IStepEvent<TSelf> : IStepEvent
        where TSelf : class, IStepEvent<TSelf>
    {
        void IStepEvent.Publish(VhId sourceId, IMessageBus messageBus)
        {
            messageBus.Publish<EventSequenceGroupEnum, VhId, TSelf>(in sourceId, Unsafe.As<TSelf>(this));
        }

        void IStepEvent.Revert(VhId sourceId, IMessageBus messageBus)
        {
            messageBus.Publish<RevertEventSequenceGroupEnum, VhId, TSelf>(in sourceId, Unsafe.As<TSelf>(this));
        }
    }
}