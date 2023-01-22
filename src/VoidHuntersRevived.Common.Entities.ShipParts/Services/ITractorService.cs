using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Services
{
    public interface ITractorService
    {
        bool TryGetTractorable(Vector2 target, out ParallelKey tractorable);
        bool CanTractor(Vector2 target, ParallelKey tractorable);
    }
}
