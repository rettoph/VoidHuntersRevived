using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities.Drivers
{
    public abstract class PlayerDriver : Driver
    {
        protected Player parent { get; private set; }

        public PlayerDriver(
            Player parent,
            EntityConfiguration configuration,
            Scene scene,
            ILogger logger) : base(configuration, scene, logger)
        {
            this.parent = parent;
        }

        public abstract void HandleUpdateBridge();
        public abstract void HandleUpdateDirection(Direction direction, Boolean value);
    }
}
