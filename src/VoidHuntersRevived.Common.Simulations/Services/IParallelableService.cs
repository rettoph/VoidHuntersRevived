using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Simulations.Components;

namespace VoidHuntersRevived.Common.Simulations.Services
{
    public interface IParallelableService
    {
        Parallelable Get(ParallelKey key);
        Parallelable Get(int id);
        bool TryGet(int id, [MaybeNullWhen(false)] out Parallelable parallelable);
    }
}
