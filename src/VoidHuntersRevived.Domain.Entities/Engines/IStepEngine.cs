using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Engines
{
    public interface IStepEngine : Svelto.ECS.IStepEngine<Step>
    {
    }
}
