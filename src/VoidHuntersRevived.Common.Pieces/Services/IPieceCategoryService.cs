using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface IPieceCategoryService
    {
        void Configure(PieceCategory type, Action<IPieceCategoryConfiguration> configuration);
    }
}
