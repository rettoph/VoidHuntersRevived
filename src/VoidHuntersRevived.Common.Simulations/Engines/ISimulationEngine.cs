using Svelto.ECS;

namespace VoidHuntersRevived.Common.Simulations.Engines
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
