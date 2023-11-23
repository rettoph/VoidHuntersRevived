using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Enums;
using Guppy.GUI;
using Guppy.MonoGame.Components;
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

namespace VoidHuntersRevived.Domain.Debugging.Components
{
    [AutoLoad]
    [GuppyFilter<IGameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.PostInitialize)]
    internal class LockstepSimulation_ClientDebugComponent : GuppyComponent, IDebugComponent
    {
        private readonly ISimulationService _simulations;
        private LockstepSimulation_Client? _client;
        private LockstepSimulation_Server? _server;
        private bool _enabled;

        public LockstepSimulation_ClientDebugComponent(ISimulationService simulations)
        {
            _simulations = simulations;
        }

        public override void Initialize(IGuppy guppy)
        {
            base.Initialize(guppy);

            if(_simulations.Flags.HasFlag(SimulationType.Lockstep) == false)
            {
                _enabled = false;
                return;
            }

            _client = _simulations[SimulationType.Lockstep] as LockstepSimulation_Client;
            _server = _simulations[SimulationType.Lockstep] as LockstepSimulation_Server;

            _enabled = true;
        }

        public void Initialize(IGui gui)
        {
            //throw new NotImplementedException();
        }

        public void RenderDebugInfo(IGui gui, GameTime gameTime)
        {
            if(_enabled == false)
            {
                return;
            }

            if (_server is not null)
            {
                gui.Text(nameof(LockstepSimulation_Server));
                gui.Indent();

                gui.Text($"Tick: {_server.CurrentTick.Id}");
                gui.Text($"Step: {_server._stepsSinceTick}/{_server._stepsPerTick}");

                gui.Unindent();
            }

            if (_client is not null)
            {
                gui.Text(nameof(LockstepSimulation_Client));
                gui.Indent();

                gui.Text($"Tick: {_client.CurrentTick.Id}");
                gui.Text($"Step: {_client._stepsSinceTick}/{_client._stepsPerTick}");
                gui.Text($"Buffer Head: {(_client._ticks.Head?.Id.ToString()) ?? "null"}");
                gui.Text($"Buffer Tail: {(_client._ticks.Tail?.Id.ToString()) ?? "null"}");
                gui.Text($"Buffer Count: {_client._ticks.Count}");

                gui.Unindent();
            }
        }
    }
}
