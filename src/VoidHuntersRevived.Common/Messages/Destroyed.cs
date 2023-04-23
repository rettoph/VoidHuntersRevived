using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Messages
{
    public class Destroyed<T> : Message<Destroyed<T>>
    {
        public readonly T Instance;

        public Destroyed(T instance)
        {
            Instance = instance;
        }
    }
}
