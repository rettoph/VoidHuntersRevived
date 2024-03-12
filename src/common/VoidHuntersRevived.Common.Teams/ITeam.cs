using Guppy.Resources;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Teams
{
    public interface ITeam
    {
        Id<ITeam> Id { get; }
        string Name { get; }
        ResourceValue<Color> PrimaryColor { get; }
        ResourceValue<Color> SecondaryColor { get; }
    }
}
