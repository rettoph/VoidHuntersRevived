﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Pieces
{
    public class Blueprint
    {
        [JsonIgnore]
        public Id<Blueprint> Id => new Id<Blueprint>(this.Piece.Hash);

        public string Name { get; set; } = string.Empty;
        public BlueprintPiece Piece { get; set; }
    }
}
