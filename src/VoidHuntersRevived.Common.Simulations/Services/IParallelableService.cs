using VoidHuntersRevived.Common.Simulations.Components;

namespace VoidHuntersRevived.Common.Simulations.Services
{
    public interface IParallelableService
    {
        Parallelable Get(ParallelKey key);
        Parallelable Get(int id);
    }
}
