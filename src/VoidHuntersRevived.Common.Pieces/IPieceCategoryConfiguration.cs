using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Common.Pieces
{
    public interface IPieceCategoryConfiguration
    {
        public PieceCategory Category { get; }

        IPieceCategoryConfiguration HasProperty<T>()
            where T : class, IPieceProperty;
    }
}
