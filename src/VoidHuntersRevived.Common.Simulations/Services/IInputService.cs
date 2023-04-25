using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Services
{
    public interface IInputService
    {
        void Publish(Input input, ISimulation simulation);

        void Rollback(Input input, ISimulation simulation);
    }
}
