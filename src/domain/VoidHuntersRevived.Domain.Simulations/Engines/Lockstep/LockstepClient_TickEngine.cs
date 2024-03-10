using Guppy.Attributes;
using Guppy.Messaging;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Serilog;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Engines.Lockstep
{
    [AutoLoad]
    [PeerFilter(PeerType.Client)]
    [SimulationFilter(SimulationType.Lockstep)]
    internal class LockstepClient_TickEngine : BasicEngine,
        ISubscriber<INetIncomingMessage<Tick>>,
        ISubscriber<INetIncomingMessage<TickHistoryStart>>,
        ISubscriber<INetIncomingMessage<TickHistoryItem>>,
        ISubscriber<INetIncomingMessage<TickHistoryEnd>>
    {
        private readonly TickBuffer _ticks;
        private readonly ILogger _logger;

        public LockstepClient_TickEngine(ILogger logger, TickBuffer ticks)
        {
            _ticks = ticks;
            _logger = logger;
        }

        public void Process(in Guid messsageId, INetIncomingMessage<Tick> message)
        {
            TickBuffer.EnqueueTickResponse response = _ticks.TryEnqueue(message.Body);
            _logger.Verbose("{ClassName}::{MethodName}<{T}> - Attempted to enqueue Tick {Id}, Response = {Response}", nameof(LockstepClient_TickEngine), nameof(Process), nameof(Tick), message.Body.Id, response);
        }

        public void Process(in Guid messsageId, INetIncomingMessage<TickHistoryStart> message)
        {
            //_ticks.Clear();
            _logger.Verbose("{ClassName}::{MethodName}<{T}> - CurrentTickId = {CurrentTickId}", nameof(LockstepClient_TickEngine), nameof(Process), nameof(TickHistoryStart), message.Body.CurrentTickId);
        }

        public void Process(in Guid messsageId, INetIncomingMessage<TickHistoryItem> message)
        {
            TickBuffer.EnqueueTickResponse response = TickBuffer.EnqueueTickResponse.NotEnqueued;
            Tick? previous = _ticks.Previous(message.Body.Tick.Id);
            int id = (previous?.Id ?? 0) + 1;

            _logger.Verbose("{ClassName}::{MethodName}<{T}> - TickId = {CurrentTickId}, PreviousTickId = {PreviousTickId}", nameof(LockstepClient_TickEngine), nameof(Process), nameof(TickHistoryItem), message.Body.Tick.Id, previous?.Id ?? 0);
            for (; id < message.Body.Tick.Id; id++)
            {
                response = _ticks.TryEnqueue(Tick.Empty(id));
                _logger.Verbose("{ClassName}::{MethodName}<{T}> - Attempted to enqueue empty Tick {TickId}, Response = {Response}", nameof(LockstepClient_TickEngine), nameof(Process), nameof(TickHistoryItem), id, response);
            }

            response = _ticks.TryEnqueue(message.Body.Tick);
            _logger.Verbose("{ClassName}::{MethodName}<{T}> - Attempted to enqueue Tick {TickId}, Response = {Response}", nameof(LockstepClient_TickEngine), nameof(Process), nameof(TickHistoryItem), id, response);
        }

        public void Process(in Guid messsageId, INetIncomingMessage<TickHistoryEnd> message)
        {
            TickBuffer.EnqueueTickResponse response = TickBuffer.EnqueueTickResponse.NotEnqueued;
            Tick? previous = _ticks.Previous(message.Body.CurrentTickId);
            int id = (previous?.Id ?? 0) + 1;

            _logger.Verbose("{ClassName}::{MethodName}<{T}> - CurrentTickId = {CurrentTickId}, PreviousId = {PreviousId}", nameof(LockstepClient_TickEngine), nameof(Process), nameof(TickHistoryEnd), message.Body.CurrentTickId, previous?.Id ?? 0);
            for (; id < message.Body.CurrentTickId; id++)
            {
                response = _ticks.TryEnqueue(Tick.Empty(id));
                _logger.Verbose("{ClassName}::{MethodName}<{T}> - Attempted to enqueue empty Tick {TickId}, Response = {Response}", nameof(LockstepClient_TickEngine), nameof(Process), nameof(TickHistoryEnd), id, response);
            }
        }
    }
}
