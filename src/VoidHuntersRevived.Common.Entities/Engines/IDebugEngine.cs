using Guppy.Game.Components;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface IDebugEngine : IDebugComponent
    {
        string Group => string.Empty;
    }
}
