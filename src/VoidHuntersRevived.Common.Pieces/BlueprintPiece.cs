using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Pieces
{
    public class BlueprintPiece
    {
        [JsonIgnore]
        public VhId Hash => HashBuilder<BlueprintPiece, VhId>.Instance.Calculate(this.Key);

        public string Key { get; set; } = string.Empty;
    }
}
