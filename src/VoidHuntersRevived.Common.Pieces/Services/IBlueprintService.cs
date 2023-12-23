using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface IBlueprintService : IEntityResourceService<IBlueprint>
    {
        // IBlueprintSpawner GetBlueprintSpawner(BlueprintDto blueprint);
        bool TryGetByDto(BlueprintDto blueprintDto, [MaybeNullWhen(false)] out IBlueprint blueprint);
    }
}
