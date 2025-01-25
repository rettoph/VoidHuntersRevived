using Guppy.Core.Messaging.Common;
using Guppy.Core.Network.Common;
using Guppy.Core.Logging.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Engines.Lockstep
{
    internal class LockstepClient_TickEngine(ILogger logger, TickBuffer ticks) : StrategyEngine<ILockstepStrategy>,
        IClientEngine,
        ISubscriber<INetIncomingMessage<Tick>>,
        ISubscriber<INetIncomingMessage<TickHistoryStart>>,
        ISubscriber<INetIncomingMessage<TickHistoryItem>>,
        ISubscriber<INetIncomingMessage<TickHistoryEnd>>
    {
        private readonly TickBuffer _ticks = ticks;
        private readonly ILogger _logger = logger;

        public void Process(in Guid messsageId, INetIncomingMessage<Tick> message)
        {
            TickBuffer.EnqueueTickResponseEnum response = this._ticks.TryEnqueue(message.Body);
            this._logger.Verbose("Attempted to enqueue Tick {Id}, Response = {Response}", message.Body.Id, response);
        }

        public void Process(in Guid messsageId, INetIncomingMessage<TickHistoryStart> message)
        {
            //_ticks.Clear();
            this._logger.Verbose("CurrentTickId = {CurrentTickId}", message.Body.CurrentTickId);
        }

        public void Process(in Guid messsageId, INetIncomingMessage<TickHistoryItem> message)
        {
            Tick? previous = this._ticks.Previous(message.Body.Tick.Id);
            int id = (previous?.Id ?? 0) + 1;

            this._logger.Verbose("TickId = {CurrentTickId}, PreviousTickId = {PreviousTickId}", message.Body.Tick.Id, previous?.Id ?? 0);
            TickBuffer.EnqueueTickResponseEnum response;
            for (; id < message.Body.Tick.Id; id++)
            {
                response = this._ticks.TryEnqueue(Tick.Empty(id));
                this._logger.Verbose("Attempted to enqueue empty Tick {TickId}, Response = {Response}", id, response);
            }

            response = this._ticks.TryEnqueue(message.Body.Tick);
            this._logger.Verbose("Attempted to enqueue Tick {TickId}, Response = {Response}", id, response);
        }

        public void Process(in Guid messsageId, INetIncomingMessage<TickHistoryEnd> message)
        {
            Tick? previous = this._ticks.Previous(message.Body.CurrentTickId);
            int id = (previous?.Id ?? 0) + 1;

            this._logger.Verbose("CurrentTickId = {CurrentTickId}, PreviousId = {PreviousId}", message.Body.CurrentTickId, previous?.Id ?? 0);
            for (; id < message.Body.CurrentTickId; id++)
            {
                TickBuffer.EnqueueTickResponseEnum response = this._ticks.TryEnqueue(Tick.Empty(id));
                this._logger.Verbose("Attempted to enqueue empty Tick {TickId}, Response = {Response}", id, response);
            }
        }
    }
}