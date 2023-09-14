using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces
{
    public class Piece
    {
        public string Key { get; set; } = string.Empty;
        public VoidHuntersEntityDescriptor Descriptor { get; set; } = null!;
        public IPieceComponent[] Components { get; set; } = Array.Empty<IPieceComponent>();
    }
}
