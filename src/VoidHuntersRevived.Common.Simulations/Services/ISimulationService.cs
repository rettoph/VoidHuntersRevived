using Guppy.Network.Enums;
using LiteNetLib;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace VoidHuntersRevived.Common.Simulations.Services
{
    public interface ISimulationService
    {
        public SimulationType Flags { get; }

        ReadOnlyCollection<ISimulation> Instances { get; }

        ISimulation this[SimulationType type] { get; }

        void Initialize(SimulationType simulationTypeFlags);

        ISimulation First(params SimulationType[] types);

        void Update(GameTime gameTime);

        void Input(InputDto input);
    }
}
