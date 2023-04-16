using Guppy.Network.Identity;
using Guppy.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using System.Reflection;
using VoidHuntersRevived.Common.Simulations.Services;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Domain.Simulations.Extensions;
using Guppy.MonoGame;
using MonoGame.Extended;
using Guppy.MonoGame.Providers;
using Guppy.MonoGame.Constants;
using Guppy.Common;
using Guppy.MonoGame.Messages;
using Guppy.Attributes;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Client.Debuggers
{
    [GuppyFilter<LocalGameGuppy>()]
    internal sealed class LockstepStateDebugger : SimpleDrawableGameComponent,
        ISubscriber<Toggle<LockstepStateDebugger>>
    {
        private readonly IGlobalSimulationService _simulations;
        private readonly Menu _menu;

        public LockstepStateDebugger(
            IGlobalSimulationService simulations, 
            IMenuProvider menus)
        {
            _simulations = simulations;
            _menu = menus.Get(MenuConstants.Debug);

            this.IsEnabled = false;
            this.Visible = this.IsEnabled;
        }

        public override void Initialize()
        {
            _menu.Add(new MenuItem()
            {
                Label = "Lockstep State",
                OnClick = Toggle<LockstepStateDebugger>.Instance
            });
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var data in _simulations.Instances.OfType<ILockstepSimulation>())
            {
                this.DrawData(data);
            }
        }

        private void DrawData(ILockstepSimulation simulation)
        {
        }

        private void DrawTick(Tick tick)
        {
        }

        public void Process(in Toggle<LockstepStateDebugger> message)
        {
            this.IsEnabled = !this.IsEnabled;
            this.Visible = this.IsEnabled;
        }
    }
}
