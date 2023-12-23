﻿using Guppy.Attributes;
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
    [SimulationFilter<LockstepSimulation_Server>]
    internal class LockstepSimulation_ServerDebugEngine : BasicEngine<LockstepSimulation_Server>, ISimpleDebugEngine
    {
        public const string Time = nameof(Time);
        public const string Tick = nameof(Tick);
        public const string Step = nameof(Step);

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public LockstepSimulation_ServerDebugEngine()
        {
            this.Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), Tick, () => this.Simulation.CurrentTick.Id.ToString("#,###,##0")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), Step, () => $"{this.Simulation.stepsSinceTick}/{this.Simulation.stepsPerTick}"),
            };
        }
    }
}
