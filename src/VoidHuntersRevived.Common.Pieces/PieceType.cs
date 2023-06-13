using Standart.Hash.xxHash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Pieces
{
    public class PieceType
    {
        public readonly string Name;
        public readonly Guid Hash;

        public unsafe PieceType(Guid nameSpace, string name)
        {
            this.Name = name;

            uint128 nameHash = xxHash128.ComputeHash(name);
            Guid* pNameHash = (Guid*)&nameHash;

            this.Hash = nameSpace.Create(pNameHash[0]);
        }
    }
}
