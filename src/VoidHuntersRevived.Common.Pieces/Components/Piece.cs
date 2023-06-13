using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Piece<T>
        where T : class, IPieceProperty
    {
        public int Value;
    }
}
