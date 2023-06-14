using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Entities.EnginesGroups
{
    public abstract class BulkEnginesGroups<TEngine, TParameter> : UnsortedEnginesGroup<TEngine, TParameter>
        where TEngine : IStepEngine<TParameter>
    {
        public BulkEnginesGroups(IEnumerable<TEngine> engines)
        {
            foreach(TEngine engine in engines)
            {
                this.Add(engine);
            }
        }

    }
}
