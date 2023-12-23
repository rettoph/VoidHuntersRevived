using Guppy;
using Guppy.Attributes;
using Guppy.Messaging;
using Guppy.Providers;
using VoidHuntersRevived.Common.Messages;

namespace VoidHuntersRevived.Domain.GameComponents
{
    [AutoLoad]
    internal sealed class LaunchComponent : IGuppyComponent,
        ISubscriber<Launch>
    {
        private readonly IGuppy _guppy;
        private readonly IGuppyProvider _guppies;

        public LaunchComponent(IBus bus, IGuppy guppy, IGuppyProvider guppies)
        {
            _guppy = guppy;
            _guppies = guppies;

            bus.Subscribe(this);
        }

        public void Initialize(IGuppy guppy)
        {
            //
        }

        public void Process(in Guid messageId, Launch message)
        {
            _guppies.Create(message.GuppyType);
            _guppy.Dispose();
        }
    }
}
