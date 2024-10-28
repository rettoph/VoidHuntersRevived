using VoidHuntersRevived.Tests.Common.Simulations;

namespace VoidHuntersRevived.Tests.Registration.Pieces.Extensions
{
    public static class SimulationBuilderExtensions
    {
        public static SimulationBuilder AddPiecesConfiguration(this SimulationBuilder builder)
        {
            builder.AddConfiguration(services => services.RegisterPiecesServices());

            return builder;
        }
    }
}
