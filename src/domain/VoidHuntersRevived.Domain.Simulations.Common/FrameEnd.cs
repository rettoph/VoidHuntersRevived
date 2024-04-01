using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public class FrameEnd
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
        public readonly Frame Frame;

        public FrameEnd(Frame frame)
        {
            _gameTime = default!;
            this.Frame = frame;
        }
    }
}
