using Guppy.Tests.Common;
using tainicom.Aether.Physics2D.Common;

namespace VoidHuntersRevived.Tests.Common.Physics.Builders
{
    public class AetherWorldBuilder : Builder<AetherWorld>
    {
        public AetherVector2 Gravity { get; set; } = AetherVector2.Zero;

        protected override AetherWorld Build()
        {
            return new AetherWorld(
                gravity: this.Gravity);
        }
    }
}
