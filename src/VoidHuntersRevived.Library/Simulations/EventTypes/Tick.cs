using Guppy.Common;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Simulations.EventData;

namespace VoidHuntersRevived.Library.Simulations.EventTypes
{
    public class Tick : EventType
    {
        public const int MinimumValidId = 1;
        public const int MaximumInvalidId = MinimumValidId - 1;

        public int Id { get; }

        internal Tick(int id, IEnumerable<ISimulationEventData> eventData) : base(eventData)
        {
            Id = id;
        }

        public static Tick Empty(int id)
        {
            return new Tick(id, Enumerable.Empty<ISimulationEventData>());
        }
    }
}
