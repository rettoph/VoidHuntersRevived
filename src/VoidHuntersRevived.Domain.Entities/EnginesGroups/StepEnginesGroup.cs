using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.EnginesGroups
{
    internal class StepEnginesGroup : BulkEnginesGroups<IStepEngine<Step>, Step>
    {
        public StepEnginesGroup(IEnumerable<IStepEngine<Step>> engines) : base(engines)
        {
        }
    }
}
