using Guppy.MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain
{
    public abstract class GameGuppy : FrameableGuppy, IGameGuppy
    {
        public readonly ISimulationService Simulations;

        protected GameGuppy(ISimulationService simulations)
        {
            this.Simulations = simulations;
        }

        public override void Initialize(IServiceProvider provider)
        {
            base.Initialize(provider);

            this.Simulations.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Simulations.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
