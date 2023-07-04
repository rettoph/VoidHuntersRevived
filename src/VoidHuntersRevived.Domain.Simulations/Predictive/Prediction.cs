using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    public class Prediction
    {
        public static readonly TimeSpan MaximumAge = TimeSpan.FromSeconds(5);

        public readonly VhId EventId;
        public readonly DateTime PredictedAt;
        public PredictionStatus Status;

        public TimeSpan Age => DateTime.Now - this.PredictedAt;
        public bool Expired => this.Age >= MaximumAge;

        public Prediction(VhId eventId)
        {
            this.EventId = eventId;
            this.PredictedAt = DateTime.Now;
            this.Status = PredictionStatus.Unverified;
        }
    }
}
