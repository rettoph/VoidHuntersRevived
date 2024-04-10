using Guppy;
using System.Reflection;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Core
{
    public class VoidHuntersEngine : GuppyEngine
    {
        private static readonly Assembly[] GameLibraries = [
            typeof(VhId).Assembly
        ];

        public VoidHuntersEngine() : base(VoidHuntersRevivedGame.Company, VoidHuntersRevivedGame.Name, GameLibraries)
        {
        }

        public VoidHuntersEngine(string suffix) : base(VoidHuntersRevivedGame.Company, VoidHuntersRevivedGame.Name + "." + suffix, GameLibraries)
        {
        }
    }
}
