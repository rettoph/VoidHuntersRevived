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
        public readonly GameStateType Type;
        public readonly int LastHistoricTickId;

        private GameState(GameStateType type, int lastHistoricTickId)
        {
            this.Type = type;
            this.LastHistoricTickId = lastHistoricTickId;
        }

        public static readonly GameState End = new GameState(GameStateType.End, default);

        public static GameState Begin(int lastHistoricTickId)
        {
            return new GameState(GameStateType.Begin, lastHistoricTickId);
        }
    }

    public enum GameStateType : byte
    {
        None,
        Begin,
        End
    }
}
