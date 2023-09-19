using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Pieces
{
    public interface IBlueprint : IEntityResource<IBlueprint>
    {
        string Name { get; }
        IBlueprintPiece Head { get; }
    }
}
