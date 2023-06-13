using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public ref struct Ref<T>
        where T : unmanaged
    {
        public ref T Instance;

        public Ref(ref T instance)
        {
            this.Instance = ref instance;
        }
    }
}
