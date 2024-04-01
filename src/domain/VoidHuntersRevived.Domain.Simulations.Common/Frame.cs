using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public class Frame
    {
        private GameTime _gameTime;

        public GameTime GameTime
        {
            get => _gameTime;
            set
            {
                _gameTime = value;
                this.UpdatedAd = DateTime.Now;
            }
        }

        public DateTime UpdatedAd { get; set; }
        public readonly FrameStart Start;

        public Frame(FrameStart start)
        {
            _gameTime = default!;
            this.Start = start;
        }
    }
}
