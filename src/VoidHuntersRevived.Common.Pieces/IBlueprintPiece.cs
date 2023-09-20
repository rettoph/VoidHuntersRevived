using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Pieces
{
    public interface IBlueprintPiece
    {
        Piece Piece { get; }

        IBlueprintPiece[][] Children { get; }
    }
}
