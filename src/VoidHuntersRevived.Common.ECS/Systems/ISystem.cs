using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.ECS.Systems
{
    public interface ISystem : IDisposable
    {
        void Initialize(IWorld world);
    }
}
