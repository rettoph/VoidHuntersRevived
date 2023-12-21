using Guppy.Network.Enums;
using LiteNetLib;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VoidHuntersRevived.Common.Simulations.Services
{
    public interface ISimulationService
    {
        SimulationType Flags { get; }

        ReadOnlyCollection<ISimulation> Instances { get; }

        ISimulation this[SimulationType type] { get; }

        void Configure(SimulationType simulationTypeFlags);

        void Initialize();

        ISimulation First(params SimulationType[] types);

        void Draw(GameTime gameTime);

        void Update(GameTime gameTime);

        void Input(VhId sender, IInputData data);
    }
}
