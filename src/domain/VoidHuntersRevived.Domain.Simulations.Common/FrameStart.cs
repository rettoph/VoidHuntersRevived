using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public class FrameStart
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

        public FrameStart()
        {
            _gameTime = default!;
        }
    }
}
