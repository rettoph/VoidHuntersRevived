using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface IPiecePropertyService
    {
        void Configure<T>(Action<IPiecePropertyConfiguration<T>> configuration)
            where T : class, IPieceProperty;
    }
}
