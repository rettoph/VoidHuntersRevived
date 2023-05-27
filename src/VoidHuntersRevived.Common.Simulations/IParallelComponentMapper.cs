using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Physics;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IParallelComponentMapper
    {

    }
    public interface IParallelComponentMapper<T> : IParallelComponentMapper
        where T : class
    {
        T Get(ParallelKey entityKey, ISimulation simulation);
        bool TryGet(ParallelKey entityKey, ISimulation simulation, [MaybeNullWhen(false)] out T value);
        bool Has(ParallelKey entityKey, ISimulation simulation);
        void Put(ParallelKey entityKey, ISimulation simulation, T component);
    }
}
