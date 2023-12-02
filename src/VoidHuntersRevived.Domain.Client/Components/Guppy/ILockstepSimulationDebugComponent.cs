using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Enums;
using Guppy.Game.Components;
using Guppy.GUI;
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
        private readonly IGui _gui;

        public LockstepSimulation_ClientDebugComponent(IGui gui, ISimulationService simulations)
        {
            _gui = gui;
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
                _gui.Text(nameof(LockstepSimulation_Server));
                _gui.Indent();

                _gui.Text($"Tick: {_server.CurrentTick.Id}");
                _gui.Text($"Step: {_server._stepsSinceTick}/{_server._stepsPerTick}");

                _gui.Unindent();
            }

            if (_client is not null)
            {
                _gui.Text(nameof(LockstepSimulation_Client));
                _gui.Indent();

                _gui.Text($"Tick: {_client.CurrentTick.Id}");
                _gui.Text($"Step: {_client._stepsSinceTick}/{_client._stepsPerTick}");
                _gui.Text($"Buffer Head: {(_client._ticks.Head?.Id.ToString()) ?? "null"}");
                _gui.Text($"Buffer Tail: {(_client._ticks.Tail?.Id.ToString()) ?? "null"}");
                _gui.Text($"Buffer Count: {_client._ticks.Count}");

                _gui.Unindent();
            }
        }
    }
}
