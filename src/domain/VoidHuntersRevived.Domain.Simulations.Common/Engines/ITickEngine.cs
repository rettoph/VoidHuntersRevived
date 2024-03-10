using Svelto.ECS;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface ITickEngine : IStepEngine<Tick>
    {
    }
}
