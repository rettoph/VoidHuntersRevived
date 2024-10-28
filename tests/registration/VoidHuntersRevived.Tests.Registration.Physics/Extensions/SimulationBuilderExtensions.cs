using VoidHuntersRevived.Tests.Common.Simulations;

namespace VoidHuntersRevived.Tests.Registration.Physics.Extensions
{
    public static class SimulationBuilderExtensions
    {
        public static SimulationBuilder AddPhysicsConfiguration(this SimulationBuilder builder)
        {
            builder.AddConfiguration(services => services.RegisterPhysicsServices());

            return builder;
        }
    }
}
