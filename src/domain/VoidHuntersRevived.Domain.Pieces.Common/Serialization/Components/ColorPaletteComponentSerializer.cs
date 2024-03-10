using Guppy.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common.Serialization.Components
{
    [AutoLoad]
    internal sealed class ColorPaletteComponentSerializer : DoNotSerializeComponentSerializer<ColorScheme>
    {
    }
}
