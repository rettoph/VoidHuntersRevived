using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces
{
    public interface IBlueprint : IEntityResource<IBlueprint>
    {
        string Name { get; }
        IBlueprintPiece Head { get; }
    }
}
