using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static class UnsortedEnginesGroupExtensions
    {
        public static UnsortedEnginesGroup<TInterface, TParameter> CreateUnsortedEnginesGroup<TInterface, TParameter>(this IEnumerable<IEngine> engines)
            where TInterface : IStepEngine<TParameter>
        {
            return new UnsortedEnginesGroupImplementation<TInterface, TParameter>(engines.OfType<TInterface>());
        }

        private class UnsortedEnginesGroupImplementation<TInterface, TParameter> : UnsortedEnginesGroup<TInterface, TParameter>
            where TInterface : IStepEngine<TParameter>
        {
            public UnsortedEnginesGroupImplementation(IEnumerable<TInterface> engines) : base(new FasterList<TInterface>(engines.ToArray()))
            {

            }
        }
    }
}
