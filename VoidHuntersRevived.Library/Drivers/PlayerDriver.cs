using Guppy.Implementations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Drivers
{
    public abstract class PlayerDriver : Driver
    {
        public PlayerDriver(Driven entity, IServiceProvider provider, ILogger logger) : base(entity, provider, logger)
        {
        }

        public virtual void ConfigurePlayer(ref TractorBeam tractorbeam)
        {

        }
    }
}
