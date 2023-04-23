using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Services
{
    public interface INodeService
    {
        void Attach(Degree outDegree, Degree inDegree);
        void Detach(Degree degree);
    }
}
