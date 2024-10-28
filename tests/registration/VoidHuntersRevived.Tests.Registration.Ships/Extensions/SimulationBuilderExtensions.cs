using VoidHuntersRevived.Tests.Common.Simulations;

namespace VoidHuntersRevived.Tests.Registration.Ships.Extensions
{
    public static class SimulationBuilderExtensions
    {
        public static SimulationBuilder AddShipsConfiguration(this SimulationBuilder builder)
        {
            builder.AddConfiguration(services => services.RegisterShipsServices());

            return builder;
        }
    }
}
