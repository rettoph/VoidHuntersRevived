using Guppy.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Messages
{
    public sealed class BodyPosition : Message, ISimulationEvent
    {
        public readonly int Id;
        public readonly Vector2 Position;

        public BodyPosition(int id, Vector2 position)
        {
            this.Id = id;
            this.Position = position;
        }
    }
}
