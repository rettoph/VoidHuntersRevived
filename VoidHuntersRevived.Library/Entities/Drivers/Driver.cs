using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.Drivers
{
    public abstract class Driver : Entity
    {
        public Driver(EntityConfiguration configuration, Scene scene, ILogger logger) : base(configuration, scene, logger)
        {
            this.SetVisible(false);
            this.SetEnabled(false);
        }
    }
}
