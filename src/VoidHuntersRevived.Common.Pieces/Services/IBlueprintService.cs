using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface IBlueprintService : IEntityResourceService<IBlueprint>
    {
        // IBlueprintSpawner GetBlueprintSpawner(BlueprintDto blueprint);
        bool TryGetByDto(BlueprintDto blueprintDto, [MaybeNullWhen(false)] out IBlueprint blueprint);
    }
}
