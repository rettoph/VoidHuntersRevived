using Guppy.MonoGame.UI;
using Guppy.MonoGame.UI.Debuggers;
using Guppy.Network.Identity;
using Guppy.Network;
using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using System.Reflection;
using Guppy.MonoGame.UI.Services;
using VoidHuntersRevived.Common.Simulations.Services;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Domain.Simulations.Extensions;

namespace VoidHuntersRevived.Domain.Client.Debuggers
{
    internal sealed class LockstepStateDebugger : SimpleDebugger, IImGuiDebugger
    {
        private readonly IGlobalSimulationService _simulations;
        private readonly IImguiObjectViewer _objectViewer;

        public string ButtonLabel => "Lockstep State";

        public LockstepStateDebugger(IGlobalSimulationService simulations, IImguiObjectViewer objectViewer)
        {
            _objectViewer= objectViewer;
            _simulations = simulations;

            this.IsEnabled = false;
            this.Visible = this.IsEnabled;
        }

        public void Initialize(ImGuiBatch imGuiBatch)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var data in _simulations.Instances.WhereAs<ISimulation, ILockstepSimulation>())
            {
                this.DrawData(data);
            }
        }

        private void DrawData(ILockstepSimulation simulation)
        {
            if (ImGui.Begin("Lockstep State - " + simulation.NetScope().Peer!.Type.ToString(), ImGuiWindowFlags.NoCollapse))
            {
                foreach (Tick tick in simulation.State.History)
                {
                    this.DrawTick(tick);
                }
            }
        }

        private void DrawTick(Tick tick)
        {
            if (ImGui.CollapsingHeader($"{tick.Id} ({tick.Count})"))
            {
                foreach (ISimulationData simulationData in tick.Data)
                {
                    _objectViewer.Render(simulationData);
                }
            }
        }

        public void Toggle()
        {
            this.IsEnabled = !this.IsEnabled;
            this.Visible = this.IsEnabled;
        }
    }
}
