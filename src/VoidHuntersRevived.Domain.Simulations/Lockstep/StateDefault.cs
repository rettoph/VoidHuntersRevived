using Guppy.Common;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Lockstep.Factories;
using VoidHuntersRevived.Common.Simulations.Lockstep.Messages;
using VoidHuntersRevived.Common.Simulations.Lockstep.Providers;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Constants;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    internal abstract class StateDefault : IState
    {
        private int _stepsPerTick;
        private Step _step;
        private Tick? _tick;
        private readonly PreTick _preTick;
        private readonly IList<Tick> _history;
        private readonly IBus _bus;
        private readonly ILogger _log;
        private readonly ITickProvider _ticks;
        private int _stepsSinceTick;

        protected Step step => _step;

        public IList<Tick> History => _history;

        public Tick? Tick => _tick;


        public StateDefault(
            IBus bus,
            IFiltered<ITickProvider> ticks,
            ISettingProvider settings,
            ILogger log)
        {
            _ticks = ticks.Instance;
            _log = log;
            _bus = bus;
            _history = new List<Tick>();
            _stepsPerTick = settings.Get<int>(Settings.StepsPerTick).Value;
            _step = new Step(settings.Get<TimeSpan>(Settings.StepInterval).Value);
            _preTick = new PreTick(this);
        }

        public abstract void Enqueue(SimulationEventData input);

        public void Enqueue(Tick tick)
        {
            _ticks.Enqueue(tick);
        }

        public virtual void Update(GameTime realTime)
        {
            while (this.CanStep(realTime))
            {
                this.Step(realTime);
                _stepsSinceTick++;
            }

            if(_stepsSinceTick != _stepsPerTick)
            {
                return;
            }

            if (!_ticks.TryDequeueNext(out _tick))
            { // We should never not get a tick on request within the server
                return;
            }

            _bus.Enqueue(_preTick);
            _bus.Enqueue(_tick);
            _stepsSinceTick = 0;

            if (_tick.Count > 0)
            {
                _history.Add(_tick);
            }
        }

        protected virtual bool CanStep(GameTime realTime)
        {
            if (_stepsSinceTick >= _stepsPerTick)
            {
                return false;
            }

            return true;
        }

        protected virtual void Step(GameTime realTime)
        {
            _bus.Enqueue(_step.Next());
        }

        public void BeginRead()
        {
            _stepsSinceTick = 0;
            _ticks.Reset();
        }

        public void Read(Tick tick)
        {
            if (_ticks.PeekTail(out Tick? tail))
            {
                this.EnsureEmptyTicksBetween(tail, tick);
            }

            _ticks.Enqueue(tick);
        }

        public void EndRead(int lastTickId)
        {
            this.Read(Tick.Empty(lastTickId));
        }

        private void EnsureEmptyTicksBetween(Tick start, Tick end)
        {
            if (start.Id > end.Id)
            {
                throw new Exception();
            }

            for (int i = start.Id + 1; i < end.Id; i++)
            {
                _ticks.Enqueue(Tick.Empty(i));
            }
        }
    }
}
