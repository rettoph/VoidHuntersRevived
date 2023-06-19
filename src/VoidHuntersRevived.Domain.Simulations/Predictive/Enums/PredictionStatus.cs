using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Simulations.Predictive.Enums
{
    public enum PredictionStatus : byte
    {
        Unverified,
        Verified,
        Pruned
    }
}
