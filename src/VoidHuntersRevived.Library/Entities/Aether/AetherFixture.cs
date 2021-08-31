using Guppy.DependencyInjection;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;

namespace VoidHuntersRevived.Library.Entities.Aether
{
    public class AetherFixture : BaseAetherWrapper<Fixture>
    {
        protected override Fixture BuildInstance(GuppyServiceProvider provider, NetworkAuthorization authorization)
        {
            throw new NotImplementedException();
        }
    }
}
