using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Enums;
using Guppy.Game.Components;
using Guppy.Game.ImGui;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Client.Components.Guppy
{
    [AutoLoad]
    [GuppyFilter<IVoidHuntersGameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.PostInitialize)]
    internal class LockstepSimulation_ClientDebugComponent : GuppyComponent, IDebugComponent
    {
        private readonly ISimulationService _simulations;
        private LockstepSimulation_Client? _client;
        private LockstepSimulation_Server? _server;
        private bool _enabled;
        private readonly IImGui _imgui;

        public LockstepSimulation_ClientDebugComponent(IImGui imgui, ISimulationService simulations)
        {
            _imgui = imgui;
            _simulations = simulations;
        }

        public override void Initialize(IGuppy guppy)
        {
            base.Initialize(guppy);

            if (_simulations.Flags.HasFlag(SimulationType.Lockstep) == false)
            {
                _enabled = false;
                return;
            }

            _client = _simulations[SimulationType.Lockstep] as LockstepSimulation_Client;
            _server = _simulations[SimulationType.Lockstep] as LockstepSimulation_Server;

            _enabled = true;
        }

        public void RenderDebugInfo(GameTime gameTime)
        {
            if (_enabled == false)
            {
                return;
            }

            if (_server is not null)
            {
                _imgui.Text(nameof(LockstepSimulation_Server));
                _imgui.Indent();

                _imgui.Text($"Tick: {_server.CurrentTick.Id}");
                _imgui.Text($"Step: {_server._stepsSinceTick}/{_server._stepsPerTick}");

                _imgui.Unindent();
            }

            if (_client is not null)
            {
                _imgui.Text(nameof(LockstepSimulation_Client));
                _imgui.Indent();

                _imgui.Text($"Tick: {_client.CurrentTick.Id}");
                _imgui.Text($"Step: {_client._stepsSinceTick}/{_client._stepsPerTick}");
                _imgui.Text($"Buffer Head: {(_client._ticks.Head?.Id.ToString()) ?? "null"}");
                _imgui.Text($"Buffer Tail: {(_client._ticks.Tail?.Id.ToString()) ?? "null"}");
                _imgui.Text($"Buffer Count: {_client._ticks.Count}");

                _imgui.Unindent();
            }
        }
    }
}
