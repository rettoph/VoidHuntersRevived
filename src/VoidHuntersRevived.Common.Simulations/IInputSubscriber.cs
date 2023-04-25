using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IInputSubscriber
    {
    }

    public interface IInputSubscriber<TInput>
        where TInput : Input
    {
        void Process(TInput input, ISimulation simulation);

        //void Rollback(TInput input, ISimulation simulation);
    }
}
