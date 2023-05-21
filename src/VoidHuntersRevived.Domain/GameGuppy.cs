using Guppy.MonoGame;
using Guppy.MonoGame.Services;
using Guppy.Network.Peers;
using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common;
using Guppy.Common;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using MonoGame.Extended.Timers;

namespace VoidHuntersRevived.Domain
{
    public abstract class GameGuppy : FrameableGuppy, IGameGuppy,
        ISubscriber<Step>
    {
        private World _world;
        private readonly GameTime _worldGameTime;

        public readonly ISimulationService Simulations;

        protected GameGuppy(ISimulationService simulations)
        {
            _world = default!;
            _worldGameTime = new GameTime();

            this.Simulations = simulations;
        }

        public override void Initialize(IServiceProvider provider)
        {
            base.Initialize(provider);

            _world = provider.GetRequiredService<World>();
            this.Bus.Subscribe(this);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _worldGameTime.ElapsedGameTime = gameTime.ElapsedGameTime;
            _worldGameTime.TotalGameTime += gameTime.ElapsedGameTime;

            this.UpdateWorld();

            this.Simulations.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _world.Draw(gameTime);

            base.Draw(gameTime);
        }

        public void Process(in Step message)
        {
            this.UpdateWorld();
        }

        private void UpdateWorld()
        {
            _world.Update(_worldGameTime);
            _worldGameTime.ElapsedGameTime = TimeSpan.Zero;
        }
    }
}
