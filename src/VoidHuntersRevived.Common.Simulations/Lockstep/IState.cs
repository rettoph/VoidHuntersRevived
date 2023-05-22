using Guppy.Common;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Serilog;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Simulations.Lockstep.Factories;
using VoidHuntersRevived.Common.Simulations.Lockstep.Messages;
using VoidHuntersRevived.Common.Simulations.Lockstep.Providers;

namespace VoidHuntersRevived.Common.Simulations.Lockstep
{
    public interface IState
    {
        public IList<Tick> History { get; }

        public Tick? Tick { get; }


        public void Enqueue(SimulationEventData input);

        public void Enqueue(Tick tick);

        public void Update(GameTime realTime);

        public void BeginRead();

        public void Read(Tick tick);

        public void EndRead(int lastTickId);
    }
}
