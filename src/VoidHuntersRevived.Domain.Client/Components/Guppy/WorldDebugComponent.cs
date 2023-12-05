using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Enums;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Client
{
    [AutoLoad]
    [GuppyFilter<IVoidHuntersGameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.PostInitialize)]
    internal class WorldDebugComponent : GuppyComponent
    {

    }
}
