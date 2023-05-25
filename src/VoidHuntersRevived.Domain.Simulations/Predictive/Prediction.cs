﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    public class Prediction
    {
        public static readonly TimeSpan MaximumAge = TimeSpan.FromSeconds(5);

        public readonly ParallelKey Key;
        public readonly ISimulationEvent Event;
        public readonly DateTime PredictedAt;
        public PredictionStatus Status;

        public TimeSpan Age => DateTime.Now - this.PredictedAt;
        public bool Expired => this.Age >= MaximumAge;

        public Prediction(ParallelKey key, ISimulationEvent @event)
        {
            this.Key = key;
            this.Event = @event;
            this.PredictedAt = DateTime.Now;
            this.Status = PredictionStatus.Unverified;
        }
    }
}
