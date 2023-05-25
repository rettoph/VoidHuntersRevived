using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Factories;

namespace VoidHuntersRevived.Domain.Physics.Factories
{
    internal sealed class SpaceFactory : ISpaceFactory
    {
        private readonly IBodyFactory _factory;

        public SpaceFactory(IBodyFactory factory)
        {
            _factory = factory;
        }

        public ISpace Create()
        {
            return new Space(_factory);
        }
    }
}
