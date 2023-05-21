using Guppy.Common;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Predictive.Services
{
    internal sealed class SimulationEventPredictionService
    {
        private readonly ILogger _logger;
        private readonly ISimulationEventPublishingService _events;
        private readonly Dictionary<ParallelKey, Prediction> _dict;
        private readonly Prediction[] _buffer;
        private int _tail;
        private int _head;

        public SimulationEventPredictionService(ILogger logger, ISimulationEventPublishingService events)
        {
            _logger = logger;
            _events = events;
            _dict = new Dictionary<ParallelKey, Prediction>();
            _buffer = new Prediction[1024];
        }

        public Prediction Predict(ISimulation simulation, SimulationEventData data)
        {
            if(_dict.TryGetValue(data.Key, out Prediction? prediction))
            {
                return prediction;
            }

            ISimulationEvent @event = _events.Publish(simulation, data);
            return this.Add(@event);
        }

        public void Verify(ISimulation simulation, ISimulationEvent verified)
        {
            if (!_dict.TryGetValue(verified.Key, out Prediction? prediction))
            {
                prediction = this.Predict(simulation, new SimulationEventData()
                {
                    Key = verified.Key,
                    SenderId = verified.SenderId,
                    Body = verified.Body
                });
            }

            prediction.Status = PredictionStatus.Verified;
        }

        public void Prune()
        {
            for(int i=_head; i != _tail; i++)
            {
                if(i >= _buffer.Length)
                {
                    i = 0;
                }

                if(!_buffer[i].Expired)
                {
                    break;
                }

                this.Prune(_buffer[i]);
                _head = (_head + 1) % _buffer.Length;
            }
        }

        private void Prune(Prediction? prediction)
        {
            if(prediction is null)
            {
                return;
            }

            if(prediction.Status == PredictionStatus.Pruned)
            {
                return;
            }

            if (prediction.Status == PredictionStatus.Unverified)
            {
                _logger.Warning($"{nameof(SimulationEventPredictionService)}::{nameof(Prune)} - Reverting '{prediction.Event.Body.GetType().Name}', '{prediction.Key.Value}'");
                _events.Revert(prediction.Event);
            }

            _dict.Remove(prediction.Key);
            prediction.Status = PredictionStatus.Pruned;
        }

        private Prediction Add(ISimulationEvent @event)
        {
            this.Prune(_buffer[_tail]);

            Prediction prediction = new Prediction(@event.Key, @event);
            _buffer[_tail] = prediction;
            _dict.Add(prediction.Key, prediction);

            _tail = (_tail + 1) % _buffer.Length;
            if (_tail == _head)
            {
                _head++;
            }

            return prediction;
        }
    }
}
