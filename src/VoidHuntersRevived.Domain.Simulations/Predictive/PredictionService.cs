using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    internal class PredictionService
    {
        private readonly ILogger _logger;
        private readonly EventPublishingService _publisher;
        private readonly Dictionary<VhId, Prediction> _dict;
        private readonly Prediction[] _buffer;
        private int _tail;
        private int _head;

        public PredictionService(ILogger logger, EventPublishingService publisher)
        {
            _logger = logger;
            _publisher = publisher;
            _dict = new Dictionary<VhId, Prediction>();
            _buffer = new Prediction[1024];
        }

        public Prediction Predict(EventDto @event)
        {
            if (_dict.TryGetValue(@event.Id, out Prediction? prediction))
            {
                return prediction;
            }

            _publisher.Publish(@event);
            _logger.Debug($"{nameof(PredictionService)}::{nameof(Predict)} - Predicting '{@event.Data.GetType().Name}', '{@event.Id.Value}'");

            return this.Add(@event);
        }

        public void Verify(EventDto verified)
        {
            if (!_dict.TryGetValue(verified.Id, out Prediction? prediction))
            {
                prediction = this.Predict(verified);
            }

            prediction.Status = PredictionStatus.Verified;

            _logger.Debug($"{nameof(PredictionService)}::{nameof(Verify)} - Verified '{prediction.Event.Data.GetType().Name}', '{prediction.Event.Id.Value}'");
        }

        public void Prune()
        {
            for (int i = _head; i != _tail; i++)
            {
                if (i >= _buffer.Length)
                {
                    i = 0;
                }

                if (!_buffer[i].Expired)
                {
                    break;
                }

                this.Prune(_buffer[i]);
                _head = (_head + 1) % _buffer.Length;
            }
        }

        private void Prune(Prediction? prediction)
        {
            if (prediction is null)
            {
                return;
            }

            if (prediction.Status == PredictionStatus.Pruned)
            {
                return;
            }

            if (prediction.Status == PredictionStatus.Unverified)
            {
                _logger.Warning($"{nameof(PredictionService)}::{nameof(Prune)} - Reverting '{prediction.Event.Data.GetType().Name}', '{prediction.Event.Id.Value}'");
                _publisher.Revert(prediction.Event);
            }

            _dict.Remove(prediction.Event.Id);
            prediction.Status = PredictionStatus.Pruned;
        }

        private Prediction Add(EventDto @event)
        {
            this.Prune(_buffer[_tail]);

            Prediction prediction = new Prediction(@event);
            _buffer[_tail] = prediction;
            _dict.Add(prediction.Event.Id, prediction);

            _tail = (_tail + 1) % _buffer.Length;
            if (_tail == _head)
            {
                _head++;
            }

            return prediction;
        }
    }
}
