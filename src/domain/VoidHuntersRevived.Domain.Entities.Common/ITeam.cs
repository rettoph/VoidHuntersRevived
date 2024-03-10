using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public interface ITeam : IEntityResource<ITeam>
    {
        string Name { get; }
        Color PrimaryColor { get; }
        Color SecondaryColor { get; }
    }
}
