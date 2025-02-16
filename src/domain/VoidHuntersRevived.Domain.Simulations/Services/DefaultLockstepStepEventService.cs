using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public class DefaultLockstepStepEventService(IMessageBus messageBus, ILogger logger) : BaseStepEventService(messageBus, logger)
    {
        private readonly List<EnqueuedStepInput> _inputs = [];

        public override void Input(EnqueuedStepInput input)
        {
            this._inputs.Add(input);
        }

        public EnqueuedStepInput[] FlushInputs()
        {
            EnqueuedStepInput[] result = this._inputs.ToArray();
            this._inputs.Clear();

            return result;
        }
    }
}
