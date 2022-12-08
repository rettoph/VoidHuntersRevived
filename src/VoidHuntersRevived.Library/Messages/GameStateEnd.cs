using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Messages
{
    public sealed class GameStateEnd : Message
    {
        public readonly int LastTickId;

        public GameStateEnd(int lastTickId)
        {
            this.LastTickId = lastTickId;
        }
    }
}
