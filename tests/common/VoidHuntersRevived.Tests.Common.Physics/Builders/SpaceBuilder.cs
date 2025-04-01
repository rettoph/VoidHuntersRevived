using Guppy.Core.Logging.Common;
using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Physics;

namespace VoidHuntersRevived.Tests.Common.Physics.Builders
{
    public class SpaceBuilder : Builder<Space>
    {
        public required AetherWorldBuilder AetherWorldBuilder { get; init; }
        public required Mocker<ILogger> LoggerMocker { get; init; }


        protected override Space Build()
        {
            return new Space(
                logger: this.LoggerMocker.Object,
                aether: this.AetherWorldBuilder.Object);
        }
    }
}
