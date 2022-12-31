using Guppy.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Simulations;
using VoidHuntersRevived.Library.Simulations.EventData;

namespace VoidHuntersRevived.Library.Messages
{
    public sealed class BodyPosition : Message, ISimulationEventData
    {
        public readonly int Id;
        public readonly Vector2 Position;
        public readonly SimulationType Simulation;

        public BodyPosition(int id, Vector2 position, SimulationType simulation)
        {
            this.Id = id;
            this.Position = position;
            this.Simulation = simulation;
        }
    }
}
