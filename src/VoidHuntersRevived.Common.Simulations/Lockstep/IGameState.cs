using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Common.Simulations.Lockstep
{
    public interface IGameState
    {
        public IList<Tick> History { get; }

        public Tick? Tick { get; }


        public void Enqueue(EventDto @event);

        public void Enqueue(Tick tick);

        public void Update(GameTime realTime);

        public void BeginRead();

        public void Read(Tick tick);

        public void EndRead(int lastTickId);
    }
}
