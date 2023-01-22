using Guppy.Common;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework.Input;
using Serilog;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Simulations.Lockstep.Messages;

namespace VoidHuntersRevived.Common.Simulations.Lockstep
{
    public sealed class State
    {
        private int _stepsSinceTick;
        private readonly int _stepsPerTick;
        private Step _step;
        private readonly PreTick _preTick;
        private readonly IList<Tick> _history;
        private readonly IBus _bus;
        private readonly ILogger _log;

        public int LastTickId { get; private set; }
        public int NextTickId { get; private set; }

        public int StepsSinceTick => _stepsSinceTick;
        public int LastStep { get; set; }


        public IList<Tick> History => _history;

        public bool Reading { get; private set; }


        public State(IBus bus, ISettingProvider settings, ILogger log)
        {
            _log = log;
            _bus = bus;
            _history = new List<Tick>();
            _stepsPerTick = settings.Get<int>(SettingConstants.StepsPerTick).Value;
            _step = new Step(settings.Get<TimeSpan>(SettingConstants.StepInterval).Value);
            _preTick = new PreTick(this);

            this.Reset();
        }

        public bool TryStep()
        {
            if(_stepsSinceTick > _stepsPerTick)
            {
                throw new InvalidOperationException();
            }

            if (_stepsSinceTick == _stepsPerTick)
            {
                return false;
            }

            _step = _step.Next();
            _bus.Enqueue(_step);
            _stepsSinceTick += 1;
            this.LastStep++;

            return true;
        }

        public bool CanTick()
        {
            if (_stepsSinceTick == _stepsPerTick)
            {
                return true;
            }

            return false;
        }

        public bool TryTick(Tick tick)
        {
            if (!this.CanTick())
            {
                _log.Verbose($"{nameof(State)}::{nameof(TryTick)} - Unable to Tick. This should be checked first.");
                return false;
            }

            if (tick.Id != this.NextTickId)
            {
                _log.Verbose($"{nameof(State)}::{nameof(TryTick)} - Incorrect tick recieved. Expected {this.NextTickId} but got {tick.Id}.");
                return false;
            }

            if (tick.Count != 0)
            {
                _history.Add(tick);
            }

            _bus.Enqueue(_preTick);
            _bus.Enqueue(tick);

            _stepsSinceTick = 0;
            this.LastTickId = this.NextTickId++;

            return true;
        }

        public void Reset()
        {
            _history.Clear();

            this.LastTickId = -1;
            this.NextTickId = 0;
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

            _log.Verbose($"{nameof(State)}::{nameof(BeginRead)}");
        }

        public void Read(Tick tick)
        {
            if (!this.Reading)
            {
                throw new Exception();
            }

            _log.Verbose($"{nameof(State)}::{nameof(Read)} - Reading TickId: {tick.Id}");

            while (this.LastTickId < tick.Id)
            {
                while (!this.CanTick())
                {
                    if(this.TryStep())
                    {

                    }
                }

                if (this.NextTickId == tick.Id)
                {
                    this.TryTick(tick);

                    break;
                }

                this.TryTick(Tick.Empty(this.NextTickId));
            }
        }

        public void EndRead(int? lastId = null)
        {
            if (!this.Reading)
            {
                throw new Exception();
            }

            if(lastId is not null && this.LastTickId < lastId.Value)
            {
                this.Read(Tick.Empty(lastId.Value));
            }

            this.Reading = false;
            _log.Verbose($"{nameof(State)}::{nameof(EndRead)} - Read up to TickId: {this.LastTickId}");
        }

        public void Read(IList<Tick> history)
        {
            this.BeginRead();

            foreach (Tick tick in history)
            {
                this.Read(tick);
            }

            this.EndRead();
        }
    }
}
