using VoidHuntersRevived.Tests.Common.Simulations;

namespace VoidHuntersRevived.Tests.Registration.Teams.Extensions
{
    public static class SimulationBuilderExtensions
    {
        public static SimulationBuilder AddTeamsConfiguration(this SimulationBuilder builder)
        {
            builder.AddConfiguration(services => services.RegisterTeamsServices());

            return builder;
        }
    }
}
