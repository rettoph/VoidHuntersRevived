using Guppy;
using VoidHuntersRevived.Game.Core;

namespace VoidHuntersRevived.Presentation.Core
{
    public class VoidHuntersEngine : GuppyEngine
    {
        public VoidHuntersEngine() : base(VoidHuntersRevivedGame.Company, VoidHuntersRevivedGame.Name)
        {
        }

        public VoidHuntersEngine(string suffix) : base(VoidHuntersRevivedGame.Company, VoidHuntersRevivedGame.Name + "." + suffix)
        {
        }
    }
}
