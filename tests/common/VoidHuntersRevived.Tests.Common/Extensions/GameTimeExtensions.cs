using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Tests.Common.Extensions
{
    public static class GameTimeExtensions
    {
        public static GameTime Reset(this GameTime gameTime)
        {
            gameTime.TotalGameTime = TimeSpan.Zero;
            gameTime.ElapsedGameTime = TimeSpan.Zero;

            return gameTime;
        }

        public static GameTime Step(this GameTime gameTime, TimeSpan interval)
        {
            gameTime.ElapsedGameTime = interval;
            gameTime.TotalGameTime += gameTime.ElapsedGameTime;
            return gameTime;
        }
    }
}
