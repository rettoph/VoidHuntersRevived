using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface IPieceConfigurationService
    {
        void Configure(PieceType type, Action<IPieceConfiguration> configuration);
    }
}
