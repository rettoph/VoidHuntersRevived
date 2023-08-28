using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Ships.Services
{
    public interface ITacticalService
    {
        void AddUse(EntityId tacticalId);
        void RemoveUse(EntityId tacticalId);
    }
}
