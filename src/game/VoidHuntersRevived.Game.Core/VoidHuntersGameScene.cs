using Guppy.Core.Common;
using Guppy.Game.Common;
using VoidHuntersRevived.Domain.Common;

namespace VoidHuntersRevived.Game.Core
{
    public abstract class VoidHuntersGameScene(IGuppyScope scope) : Scene(scope), IVoidHuntersGameScene
    {
    }
}