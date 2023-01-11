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
        public SimulationType Flags { get; }
        ReadOnlyCollection<ISimulation> Instances { get; }

        ISimulation this[SimulationType type] { get; }

        void Initialize(SimulationType simulationTypeFlags);

        void Update(GameTime gameTime);

        void PublishEvent(ISimulationData data, Confidence confidence);
    }
}
