using Standart.Hash.xxHash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces
{
    public class PieceCategory : EntityName
    {
        public PieceCategory(VhId nameSpace, string name) : base(nameSpace, name)
        {
        }
    }
}
