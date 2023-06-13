using Standart.Hash.xxHash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Pieces
{
    public class PieceCategory : EntityType
    {
        public PieceCategory(Guid nameSpace, string name) : base(nameSpace, name)
        {
        }
    }
}
