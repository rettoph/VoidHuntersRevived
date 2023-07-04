using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    internal class PredictionService
    {
        private readonly ILogger _logger;
        private readonly IEventPublishingService _publisher;
        private readonly Dictionary<VhId, Prediction> _predictions;
        private readonly Prediction[] _buffer;
        private int _tail;
        private int _head;

        public PredictionService(ILogger logger, IEventPublishingService publisher)
        {
            _logger = logger;
            _publisher = publisher;
            _predictions = new Dictionary<VhId, Prediction>();
            _buffer = new Prediction[1024];
        }

        public Prediction Predict(EventDto @event)
        {
            if (_predictions.TryGetValue(@event.Id, out Prediction? prediction))
            {
                return prediction;
            }

            _logger.Debug($"{nameof(PredictionService)}::{nameof(Predict)} - Predicting '{@event.Data.GetType().Name}', '{@event.Id.Value}'");
            _publisher.Publish(@event, EventValidity.Unknown);

            return this.Add(@event.Id);
        }

        public void Verify(EventDto verified)
        {
            _logger.Debug($"{nameof(PredictionService)}::{nameof(Verify)} - Verifying '{verified.Data.GetType().Name}', '{verified.Id.Value}'");

            if (_predictions.TryGetValue(verified.Id, out Prediction? prediction))
            {
                _publisher.Validate(verified.Id);
                prediction.Status = PredictionStatus.Verified;
            }
            else
            {
                _publisher.Publish(verified, EventValidity.Valid);
            }
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
                _logger.Warning($"{nameof(PredictionService)}::{nameof(Prune)} - Pruning '{prediction.EventId.Value}'");
                _publisher.Revert(prediction.EventId);
            }

            _predictions.Remove(prediction.EventId);
            prediction.Status = PredictionStatus.Pruned;
        }

        private Prediction Add(VhId eventId)
        {
            this.Prune(_buffer[_tail]);

            Prediction prediction = new Prediction(eventId);
            _buffer[_tail] = prediction;
            _predictions.Add(prediction.EventId, prediction);

            _tail = (_tail + 1) % _buffer.Length;
            if (_tail == _head)
            {
                _head++;
            }

            return prediction;
        }
    }
}
