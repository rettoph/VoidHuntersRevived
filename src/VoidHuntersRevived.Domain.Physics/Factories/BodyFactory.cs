using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Factories;

namespace VoidHuntersRevived.Domain.Physics.Factories
{
    internal sealed class BodyFactory : IBodyFactory
    {
        public IBody Create(Guid id)
        {
            return new Body(id);
        }
    }
}
