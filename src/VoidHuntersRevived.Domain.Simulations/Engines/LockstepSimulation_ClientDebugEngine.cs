using Guppy.Attributes;
using Guppy.Game.ImGui;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Engines
{
    [AutoLoad]
    [SimulationFilter<LockstepSimulation_Client>]
    internal class LockstepSimulation_ClientDebugEngine : BasicEngine<LockstepSimulation_Client>, ISimpleDebugEngine
    {
        public const string Time = nameof(Time);
        public const string Tick = nameof(Tick);
        public const string Step = nameof(Step);
        public const string BufferHead = nameof(BufferHead);
        public const string BufferTail = nameof(BufferTail);
        public const string BufferCount = nameof(BufferCount);

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public LockstepSimulation_ClientDebugEngine()
        {
            this.Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), Tick, () => this.Simulation.CurrentTick.Id.ToString("#,###,##0")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), Step, () => $"{this.Simulation._stepsSinceTick}/{this.Simulation._stepsPerTick}"),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), BufferHead, () => $"{(this.Simulation._ticks.Head?.Id.ToString()) ?? "null"}"),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), BufferTail, () => $"{(this.Simulation._ticks.Tail?.Id.ToString()) ?? "null"}"),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), BufferCount, () => this.Simulation._ticks.Count.ToString("#,##0")),
            };
        }
    }
}
