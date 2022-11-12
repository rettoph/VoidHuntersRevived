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
        public int NextTickId;
        public IEnumerable<Tick> History;

        public GameState(int nextTickId, IEnumerable<Tick> history)
        {
            this.NextTickId = nextTickId;
            this.History = history;
        }
    }
}
