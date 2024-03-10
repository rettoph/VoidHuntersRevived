using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Common.Entities
{
    public interface ITeam : IEntityResource<ITeam>
    {
        string Name { get; }
        Color PrimaryColor { get; }
        Color SecondaryColor { get; }
    }
}
