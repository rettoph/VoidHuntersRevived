using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface ISimulationEngine<in TSimulation> : IEngine
        where TSimulation : ISimulation
    {
        void Initialize(TSimulation simulation);
    }

    public interface ISimulationEngine : ISimulationEngine<ISimulation>
    {

    }
}
