using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    public sealed class Prediction
    {
        public readonly ISimulationData Data;
        public readonly DateTime CreatedAt;

        public Prediction(ISimulationData data)
        {
            this.Data = data;
            this.CreatedAt = DateTime.Now;
        }
    }
}
