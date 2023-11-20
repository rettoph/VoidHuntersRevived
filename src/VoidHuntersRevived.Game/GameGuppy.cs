using Guppy.MonoGame;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common;
using Autofac;

namespace VoidHuntersRevived.Game
{
    public abstract class GameGuppy : FrameableGuppy, IGameGuppy
    {
        public readonly ISimulationService Simulations;

        protected GameGuppy(ISimulationService simulations)
        {
            this.Simulations = simulations;
        }

        public override void Initialize(ILifetimeScope provider)
        {
            base.Initialize(provider);

            this.Simulations.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Simulations.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            this.Simulations.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
