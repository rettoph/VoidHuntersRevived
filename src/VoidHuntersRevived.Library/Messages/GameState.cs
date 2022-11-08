using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Messages
{
    /// <summary>
    /// Simple message containing the required current world data
    /// for new clients to begin reciving live tick updates.
    /// </summary>
    public class GameState
    {
        public uint NextTickId;
        public IEnumerable<Tick> History;

        public GameState(uint nextTickId, IEnumerable<Tick> history)
        {
            this.NextTickId = nextTickId;
            this.History = history;
        }
    }
}
