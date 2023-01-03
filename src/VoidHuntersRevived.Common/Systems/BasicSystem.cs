using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Systems
{
    public abstract class BasicSystem : ISystem
    {
        public virtual void Initialize(World world)
        {
        }

        public virtual void Dispose()
        {
        }
    }
}
