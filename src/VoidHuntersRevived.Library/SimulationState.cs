﻿using Guppy.Common;
using Guppy.Resources.Providers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Simulations.EventData;
using VoidHuntersRevived.Library.Simulations.EventTypes;

namespace VoidHuntersRevived.Library
{
    public class SimulationState
    {
        private int _stepsSinceTick;
        private readonly int _stepsPerTick;
        private readonly Step _step;
        private readonly IList<Tick> _history;
        private readonly IBus _bus;
        private readonly ILogger _log;

        public int LastTickId { get; private set; }
        public int NextTickId { get; private set; }

        public int StepsSinceTick => _stepsSinceTick;
        public int LastStep { get; set; }


        public IList<Tick> History => _history;

        public bool Reading { get; private set; }


        public SimulationState(IBus bus, ISettingProvider settings, ILogger log)
        {
            _log = log;
            _bus = bus;
            _history = new List<Tick>();
            _stepsPerTick = settings.Get<int>(SettingConstants.StepsPerTick).Value;
            _step = new Step(settings.Get<TimeSpan>(SettingConstants.StepInterval).Value);

            this.Reset();
        }

        public bool TryStep()
        {
            if(_stepsSinceTick >= _stepsPerTick)
            {
                return false;
            }

            _bus.Publish(_step.Increment());
            _stepsSinceTick += 1;
            this.LastStep++;

            return true;
        }

        public bool CanTick()
        {
            if(_stepsSinceTick != _stepsPerTick)
            {
                return false;
            }

            return true;
        }

        public bool TryTick(Tick tick)
        {
            if(!this.CanTick())
            {
                _log.Verbose($"{nameof(SimulationState)}::{nameof(TryTick)} - Unable to Tick. This should be checked first.");
                return false;
            }

            if (tick.Id != this.NextTickId)
            {
                _log.Verbose($"{nameof(SimulationState)}::{nameof(TryTick)} - Incorrect tick recieved. Expected {this.NextTickId} but got {tick.Id}.");
                return false;
            }

            if (tick.Count == 0)
            {
                _bus.Publish(tick);
            }
            else
            { // Only cache historical ticks if something actually happened
                _history.Add(tick);
                _bus.Publish(tick);

                foreach(ISimulationEventData eventData in tick.EventData)
                {
                    _bus.Publish(eventData);
                }
            }

            _stepsSinceTick = 0;
            this.LastTickId = this.NextTickId++;

            return true;
        }

        public void Reset()
        {
            _history.Clear();

            this.LastTickId = Tick.MaximumInvalidId;
            this.NextTickId = Tick.MinimumValidId;
            _stepsSinceTick = 0;

            this.Reading = false;
        }

        public void BeginRead()
        {
            if (this.Reading)
            {
                throw new Exception();
            }

            this.Reset();
            this.Reading = true;

            _log.Verbose($"{nameof(SimulationState)}::{nameof(BeginRead)}");
        }

        public void Read(Tick tick)
        {
            if(!this.Reading)
            {
                throw new Exception();
            }

            _log.Verbose($"{nameof(SimulationState)}::{nameof(Read)} - Reading TickId: {tick.Id}");

            while(this.LastTickId < tick.Id)
            {
                while(!this.CanTick())
                {
                    this.TryStep();
                }

                if(this.NextTickId == tick.Id)
                {
                    this.TryTick(tick);

                    break;
                }

                this.TryTick(Tick.Empty(this.NextTickId));
            }
        }

        public void EndRead()
        {
            if (!this.Reading)
            {
                throw new Exception();
            }

            this.Reading = false;
            _log.Verbose($"{nameof(SimulationState)}::{nameof(EndRead)} - Read up to TickId: {this.LastTickId}");
        }

        public void Read(IList<Tick> history)
        {
            this.BeginRead();
            
            foreach(Tick tick in history)
            {
                this.Read(tick);
            }

            this.EndRead();
        }
    }
}
