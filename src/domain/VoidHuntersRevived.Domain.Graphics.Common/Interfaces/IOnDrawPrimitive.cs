using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Interfaces
{
    public interface IOnDrawPrimitive
    {
        [RequireSequenceGroup<PrimitiveSequenceGroupEnum>]
        void OnDraw(IDrawPrimitiveContext context);
    }
}
