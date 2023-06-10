using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.ECS.Factories
{
    public interface IWorldFactory
    {
        IWorld Create(params IState[] states);
    }
}
