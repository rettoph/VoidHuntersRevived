using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Physics.Systems
{
    public interface IPhysicsSystem
    {
        void Initialize(ISpace space);
    }
}
