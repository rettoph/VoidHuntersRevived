using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    internal sealed class SpaceEngine : BasicEngine,
        IStepEngine<Step>
    {
        public string name { get; } = nameof(SpaceEngine);

        public void Step(in Step _param)
        {
            this.Simulation.Space.Step(_param);
        }
    }
}
