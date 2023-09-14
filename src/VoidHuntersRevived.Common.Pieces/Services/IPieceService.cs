using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Descriptors;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface IPieceService
    {
        Piece[] All<TDescriptor>()
            where TDescriptor : PieceDescriptor;

        Piece[] All();
    }
}
