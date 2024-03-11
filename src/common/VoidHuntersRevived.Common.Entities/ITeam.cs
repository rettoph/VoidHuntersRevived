using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Common.Entities
{
    public interface ITeam
    {
        Id<ITeam> Id { get; }
        string Name { get; }
        Color PrimaryColor { get; }
        Color SecondaryColor { get; }
    }
}
