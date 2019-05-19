using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;

namespace VoidHuntersRevived.Library.Entities.Drivers
{
    public abstract class FarseerEntityDriver : Driver
    {
        protected FarseerEntity parent { get; private set; }

        public FarseerEntityDriver(
            FarseerEntity parent,
            EntityConfiguration configuration,
            Scene scene,
            ILogger logger) : base(configuration, scene, logger)
        {
            this.parent = parent;
        }
    }
}
