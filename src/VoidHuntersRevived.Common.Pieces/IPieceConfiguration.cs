using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Pieces
{
    public interface IPieceConfiguration
    {
        PieceType Type { get; }
        PieceCategory Category { get; set; }
        IEnumerable<PieceProperty> Properties { get; }

        IPieceConfiguration SetCategory(PieceCategory category);

        IPieceConfiguration AddProperty<T>(T property)
            where T : class, IPieceProperty;
    }
}
