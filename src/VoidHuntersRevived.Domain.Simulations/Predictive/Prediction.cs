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
        public readonly int Id;
        public readonly IData Data;
        public readonly DateTime CreatedAt;

        public Prediction(int id, IData data)
        {
            this.Id = id;
            this.Data = data;
            this.CreatedAt = DateTime.Now;
        }
    }
}
