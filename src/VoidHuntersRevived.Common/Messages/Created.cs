using Guppy.Common;
using Guppy.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Messages
{
    public class Created<T> : Message<Created<T>>
    {
        public readonly T Instance;

        public Created(T instance)
        {
            Instance = instance;
        }
    }
}
